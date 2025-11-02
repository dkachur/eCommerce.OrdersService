using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Infrastructure.Caching.Interfaces;
using eCommerce.OrdersService.Infrastructure.Caching.Keys;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.Caching.Services;

public class ProductCacheService : IProductCacheService
{
    private readonly ICacheService _cache;
    private readonly ILogger<ProductCacheService> _logger;

    private static readonly TimeSpan ProductInfoTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ProductExistsTtl = TimeSpan.FromMinutes(1);

    public ProductCacheService(ICacheService cache, ILogger<ProductCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task CacheProductInfoAsync(ProductDto product, CancellationToken ct = default)
    { 
        await _cache.SetAsync(ProductCacheKeys.CreateInfoKey(product.Id), product, ProductInfoTtl, ct).ConfigureAwait(false);
        _logger.LogDebug("Cached product info for {ProductId}", product.Id);
    }
    
    public async Task CacheProductExistsAsync(Guid productId, bool exists, CancellationToken ct = default)
    {
        await _cache.SetAsync(ProductCacheKeys.CreateExistsKey(productId), exists, ProductExistsTtl, ct).ConfigureAwait(false);
        _logger.LogDebug("Cached product exists flag for {ProductId}: {Exists}", productId, exists);
    }

    public async Task CacheProductInfosAsync(IEnumerable<ProductDto> products, CancellationToken ct = default)
    {
        var productList = products.ToList();
        await _cache.SetManyAsync(
            productList.ToDictionary(
                product => ProductCacheKeys.CreateInfoKey(product.Id),
                product => product),
            ProductInfoTtl, ct).ConfigureAwait(false);

        _logger.LogDebug("Cached {Count} product infos", productList.Count);
    }

    public async Task CacheProductsExistsAsync(Dictionary<Guid, bool> exists, CancellationToken ct = default)
    {
        await _cache.SetManyAsync(
            exists.ToDictionary(
                kvp => ProductCacheKeys.CreateExistsKey(kvp.Key),
                kvp => kvp.Value),
            ProductExistsTtl, ct).ConfigureAwait(false);

        _logger.LogDebug("Cached exists flags for {Count} products", exists.Count);
    }

    public Task<ProductDto?> GetProductInfoAsync(Guid productId, CancellationToken ct = default)
        => _cache.GetAsync<ProductDto>(ProductCacheKeys.CreateInfoKey(productId), ct);
    

    public Task<bool?> GetProductExistsAsync(Guid productId, CancellationToken ct = default)
        => _cache.GetAsync<bool?>(ProductCacheKeys.CreateExistsKey(productId), ct);
    
    public async Task<Dictionary<Guid, ProductDto?>> GetProductInfosAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        var keys = productIds.Select(id => ProductCacheKeys.CreateInfoKey(id));
        var cached = await _cache.GetManyAsync<ProductDto>(keys, ct).ConfigureAwait(false);

        return cached.ToDictionary(
            kvp => ProductCacheKeys.ExtractIdFromInfoKey(kvp.Key),
            kvp => kvp.Value);
    }

    public async Task<Dictionary<Guid, bool?>> GetProductsExistsAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        var keys = productIds.Select(id => ProductCacheKeys.CreateExistsKey(id));
        var cached = await _cache.GetManyAsync<bool?>(keys, ct).ConfigureAwait(false);

        return cached.ToDictionary(
            kvp => ProductCacheKeys.ExtractIdFromExistsKey(kvp.Key),
            kvp => kvp.Value);
    }

    public async Task RemoveProductAsync(Guid productId, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(ProductCacheKeys.CreateInfoKey(productId), ct).ConfigureAwait(false);
        await _cache.SetAsync(ProductCacheKeys.CreateExistsKey(productId), false, ProductExistsTtl, ct).ConfigureAwait(false);
        _logger.LogInformation("Removed product {ProductId} from cache and marked as non-existent", productId);
    }
}
