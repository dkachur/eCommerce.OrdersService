using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.Caching.Interfaces;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Products;

public class CachedProductsServiceClient : IProductsServiceClient
{
    private readonly IProductsServiceClient _inner;
    private readonly IProductCacheService _cache;
    private readonly ILogger<CachedProductsServiceClient> _logger;

    public CachedProductsServiceClient(
        IProductsServiceClient inner, 
        IProductCacheService productCache, 
        ILogger<CachedProductsServiceClient> logger)
    {
        _inner = inner;
        _cache = productCache;
        _logger = logger;
    }

    public async Task<Dictionary<Guid, bool>> CheckProductsExistAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var cached = await _cache.GetProductsExistsAsync(ids, ct).ConfigureAwait(false);

        var uncachedIds = cached
            .Where(kvp => kvp.Value is null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (!uncachedIds.Any())
        {
            _logger.LogDebug("All {Count} product exists flags loaded from cache", cached.Count);

            return cached.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Value);
        }

        _logger.LogDebug("Cache miss for {Count} product exists checks", uncachedIds.Count);

        var freshResults = await _inner.CheckProductsExistAsync(uncachedIds, ct).ConfigureAwait(false);

        await _cache.CacheProductsExistsAsync(freshResults, ct).ConfigureAwait(false);

        var result = cached.ToDictionary(
            kvp => kvp.Key,
            kvp =>
            {
                if (kvp.Value is not null)
                    return (bool)kvp.Value;
                if (freshResults.TryGetValue(kvp.Key, out var value))
                    return value;

                _logger.LogWarning("Product {ProductId} not found in inner service results", kvp.Key);

                return default; // treat as non-existent if missing in both cache and fresh results
            });

        return result;
    }

    public async Task<List<ProductDto>> GetProductInfosAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        var cached = await _cache.GetProductInfosAsync(productIds, ct).ConfigureAwait(false);

        var uncachedIds = cached
            .Where(kvp => kvp.Value is null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (!uncachedIds.Any())
        {
            _logger.LogDebug("All {Count} product infos loaded from cache", cached.Count);
            return cached.Values.OfType<ProductDto>().ToList();
        }

        _logger.LogDebug("Cache miss for {Count} product infos", uncachedIds.Count);

        var freshResults = await _inner.GetProductInfosAsync(uncachedIds, ct).ConfigureAwait(false);
        await _cache.CacheProductInfosAsync(freshResults, ct).ConfigureAwait(false);

        var freshDict = freshResults
            .ToDictionary(p => p.Id, p => p);

        var result = cached.Select(kvp =>
        {
            if (kvp.Value is not null)
                return kvp.Value;

            if (freshDict.TryGetValue(kvp.Key, out var value))
                return value;

            _logger.LogWarning("Product {ProductId} not found in inner service results", kvp.Key);
            return null;
        }).OfType<ProductDto>().ToList();

        return result;
    }
}
