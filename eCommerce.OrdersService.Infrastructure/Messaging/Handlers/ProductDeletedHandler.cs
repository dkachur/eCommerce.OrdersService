using eCommerce.OrdersService.Infrastructure.Caching.Interfaces;
using eCommerce.OrdersService.Infrastructure.Messaging.DTOs;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Handlers;

public class ProductDeletedHandler : IMessageHandler<ProductDeletedMessage>
{
    private readonly ILogger<ProductDeletedHandler> _logger;
    private readonly IProductCacheService _cache;

    public ProductDeletedHandler(ILogger<ProductDeletedHandler> logger, IProductCacheService cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task HandleAsync(ProductDeletedMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation("Product deleted: {ProductId}", message.ProductId);
        await _cache.RemoveProductAsync(message.ProductId, ct).ConfigureAwait(false);
    }
}
