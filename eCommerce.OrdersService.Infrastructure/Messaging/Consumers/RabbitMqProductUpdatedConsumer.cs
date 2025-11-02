using eCommerce.OrdersService.Infrastructure.Messaging.Constants;
using eCommerce.OrdersService.Infrastructure.Messaging.DTOs;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using eCommerce.OrdersService.Infrastructure.Messaging.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Consumers;

public class RabbitMqProductUpdatedConsumer : RabbitMqConsumerBase<ProductUpdatedMessage>
{
    private readonly RabbitMqOptions _options;

    public RabbitMqProductUpdatedConsumer(
        IRabbitMqConnectionManager connectionManager,
        ILogger<RabbitMqProductUpdatedConsumer> logger,
        IMessageHandler<ProductUpdatedMessage> messageHandler,
        IOptions<RabbitMqOptions> options)
        : base(connectionManager, logger, messageHandler)
    {
        _options = options.Value;
    }

    protected override void Configure()
    {
        _exchange = _options.ProductsExchange;
        _routingKey = _options.ProductUpdatedRoutingKey;
        _queue = RabbitMqConstants.ProductUpdatedQueue;
    }
}
