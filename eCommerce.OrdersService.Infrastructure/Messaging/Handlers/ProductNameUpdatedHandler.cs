using eCommerce.OrdersService.Infrastructure.Messaging.DTO;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Handlers;

public class ProductNameUpdatedHandler : IMessageHandler<ProductNameUpdatedMessage>
{
    private readonly ILogger<ProductNameUpdatedHandler> _logger;

    public ProductNameUpdatedHandler(ILogger<ProductNameUpdatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ProductNameUpdatedMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation("Product name updated: {ProductId} -> {NewName}", message.ProductId, message.NewName);
        return Task.CompletedTask;
    }
}
