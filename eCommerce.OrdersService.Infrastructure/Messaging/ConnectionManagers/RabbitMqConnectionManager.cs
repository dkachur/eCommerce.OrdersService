using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using eCommerce.OrdersService.Infrastructure.Messaging.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace eCommerce.OrdersService.Infrastructure.Messaging.ConnectionManagers;

public class RabbitMqConnectionManager : IRabbitMqConnectionManager
{
    private IConnection? _connection;
    private bool _disposed;
    private readonly ConnectionFactory _factory;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private const int MaxConnectionRetries = 10;
    private static readonly TimeSpan DelayBetweenRetries = TimeSpan.FromSeconds(5);

    public RabbitMqConnectionManager(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnectionManager> logger)
    {
        _logger = logger;

        var opt = options.Value;

        _factory = new ConnectionFactory()
        {
            HostName = opt.Host,
            Port = opt.Port,
            UserName = opt.Username,
            Password = opt.Password,
        };

        _retryPolicy = CreateInitializationRetryPolicy();
    }

    public IConnection Connection
    {
        get
        {
            if (_connection is null || !_connection.IsOpen)
                throw new InvalidOperationException("RabbitMQ connection is not open");
            return _connection;
        }
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Trying to connect to RabbitMQ...");
            _connection = await _factory.CreateConnectionAsync(ct);
            _logger.LogInformation("Connected to RabbitMQ successfully");
        });
    }

    public async Task<IChannel> CreateChannel()
    {
        if (_connection is null || !_connection.IsOpen)
            await InitializeAsync();

        return await _connection!.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _disposed = true;

        GC.SuppressFinalize(this);
    }

    private AsyncRetryPolicy CreateInitializationRetryPolicy()
        => Policy
            .Handle<BrokerUnreachableException>()
            .Or<ConnectFailureException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(
                retryCount: MaxConnectionRetries,
                sleepDurationProvider: _ => DelayBetweenRetries,
                onRetry: (ex, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        ex,
                        "RabbitMQ not ready. Retry {RetryCount}/{MaxRetries} in {Delay}s",
                        retryCount,
                        MaxConnectionRetries,
                        timeSpan.TotalSeconds);
                });
}
