using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.Infrastructure.Caching.Interfaces;

public interface IProductCacheService
{
    Task CacheProductInfoAsync(ProductDto product, CancellationToken ct = default);
    Task CacheProductExistsAsync(Guid productId, bool exists, CancellationToken ct = default);

    Task CacheProductInfosAsync(IEnumerable<ProductDto> products, CancellationToken ct = default);
    Task CacheProductsExistsAsync(Dictionary<Guid, bool> exists, CancellationToken ct = default);

    Task<ProductDto?> GetProductInfoAsync(Guid productId, CancellationToken ct = default);
    Task<bool?> GetProductExistsAsync(Guid productId, CancellationToken ct = default);

    Task<Dictionary<Guid, ProductDto?>> GetProductInfosAsync(IEnumerable<Guid> productIds, CancellationToken ct = default);
    Task<Dictionary<Guid, bool?>> GetProductsExistsAsync(IEnumerable<Guid> productIds, CancellationToken ct = default);

    Task RemoveProductAsync(Guid productId, CancellationToken ct = default);
}
