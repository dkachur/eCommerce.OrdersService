using eCommerce.OrdersService.Infrastructure.Messaging.Constants;
using eCommerce.OrdersService.Infrastructure.Messaging.DTO;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using eCommerce.OrdersService.Infrastructure.Messaging.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Consumers;

public class RabbitMqProductNameUpdatedConsumer : RabbitMqConsumerBase<ProductNameUpdatedMessage>
{
    private readonly RabbitMqOptions _options;

    public RabbitMqProductNameUpdatedConsumer(
        IRabbitMqConnectionManager connectionManager,
        ILogger<RabbitMqProductNameUpdatedConsumer> logger,
        IMessageHandler<ProductNameUpdatedMessage> messageHandler,
        IOptions<RabbitMqOptions> options)
        : base(connectionManager, logger, messageHandler)
    {
        _options = options.Value;
    }

    protected override void Configure()
    {
        _exchange = _options.ProductsExchange;
        _routingKey = _options.ProductNameUpdatedRoutingKey;
        _queue = RabbitMqConstants.ProductNameUpdatedQueue;
    }
}
