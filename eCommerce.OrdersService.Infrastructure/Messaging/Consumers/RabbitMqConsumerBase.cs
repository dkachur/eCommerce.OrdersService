using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Consumers;

public abstract class RabbitMqConsumerBase<TMessage> : IMessageConsumer<TMessage>
{
    protected readonly IRabbitMqConnectionManager _connectionManager;
    protected readonly ILogger _logger;
    protected readonly IMessageHandler<TMessage> _messageHandler;

    protected IChannel? _channel;
    protected bool _disposed;
    protected CancellationToken _cancellationToken;
    protected string _exchange = string.Empty;
    protected string _routingKey = string.Empty;
    protected string _queue = string.Empty;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public RabbitMqConsumerBase(
        IRabbitMqConnectionManager connectionManager,
        ILogger logger,
        IMessageHandler<TMessage> messageHandler)
    {
        _connectionManager = connectionManager;
        _logger = logger;
        _messageHandler = messageHandler;
    }

    protected abstract void Configure();

    public async Task StartConsumingAsync(CancellationToken ct = default)
    {
        Configure();

        _cancellationToken = ct;
        _channel = await GetOrCreateChannelAsync(ct);

        await DeclareAndBindQueueAsync(_channel, ct);
        await CreateAndConfigureConsumer(_channel, ct);
    }

    private async Task OnMessageRecievedAsync(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel is null)
        {
            _logger.LogError("Channel is not initialized");
            return;
        }

        try
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<TMessage>(json, JsonOptions);
            if (message is null)
            {
                _logger.LogWarning("Received null or invalid message from queue {Queue}", _queue);
                await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                return;
            }

            _logger.LogInformation("Received message: {Message}", json);

            await _messageHandler.HandleAsync(message, _cancellationToken);

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from queue {Queue}", _queue);
            await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
        }
    }

    private async Task<IChannel> GetOrCreateChannelAsync(CancellationToken ct = default)
    {
        if (_channel is null || _channel.IsClosed)
        {
            _channel = await _connectionManager.CreateChannel();
            await _channel.ExchangeDeclareAsync(
                exchange: _exchange,
                type: ExchangeType.Direct,
                durable: true,
                cancellationToken: ct);
        }

        return _channel;
    }

    private async Task DeclareAndBindQueueAsync(IChannel channel, CancellationToken ct = default)
    {
        await channel.QueueDeclareAsync(
            queue: _queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: _queue,
            exchange: _exchange,
            routingKey: _routingKey,
            cancellationToken: ct);
    }

    private async Task CreateAndConfigureConsumer(IChannel channel, CancellationToken ct = default)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += OnMessageRecievedAsync;
        await channel.BasicConsumeAsync(
            queue: _queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        if (_channel is not null)
        {
            _logger.LogInformation("Disposing RabbitMQ consumer channel for {Queue}", _queue);
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
