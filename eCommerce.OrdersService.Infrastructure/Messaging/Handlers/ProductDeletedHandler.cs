using eCommerce.OrdersService.Infrastructure.Messaging.DTO;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Handlers;

public class ProductDeletedHandler : IMessageHandler<ProductDeletedMessage>
{
    private readonly ILogger<ProductDeletedHandler> _logger;

    public ProductDeletedHandler(ILogger<ProductDeletedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ProductDeletedMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation("Product deleted: {ProductId}", message.ProductId);
        return Task.CompletedTask;
    }
}
