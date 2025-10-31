using eCommerce.OrdersService.Infrastructure.Messaging.Constants;
using eCommerce.OrdersService.Infrastructure.Messaging.DTO;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using eCommerce.OrdersService.Infrastructure.Messaging.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Consumers;

public class RabbitMqProductDeletedConsumer : RabbitMqConsumerBase<ProductDeletedMessage>
{
    private readonly RabbitMqOptions _options;

    public RabbitMqProductDeletedConsumer(
        IRabbitMqConnectionManager connectionManager,
        ILogger<RabbitMqProductDeletedConsumer> logger,
        IMessageHandler<ProductDeletedMessage> messageHandler,
        IOptions<RabbitMqOptions> options)
        : base(connectionManager, logger, messageHandler)
    {
        _options = options.Value;
    }

    protected override void Configure()
    {
        _exchange = _options.ProductsExchange;
        _routingKey = _options.ProductDeletedRoutingKey;
        _queue = RabbitMqConstants.ProductDeletedQueue;
    }
}
