using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Products;

public class CachedProductsServiceClient : IProductsServiceClient
{
    private readonly IProductsServiceClient _inner;
    private readonly ICacheService _cache;
    private readonly ILogger<CachedProductsServiceClient> _logger;
    private const string ExistsKeyPrefix = "product-exists:";
    private const string InfoKeyPrefix = "product-info:";
    private readonly TimeSpan ExistsTtl = TimeSpan.FromMinutes(1);
    private readonly TimeSpan InfoTtl = TimeSpan.FromMinutes(5);
    private const string NoInfo = "No Info";
    private static readonly ProductDto FallbackProductTemplate = new(
        Guid.Empty,
        NoInfo,
        NoInfo,
        default,
        default);

    public CachedProductsServiceClient(IProductsServiceClient inner, ICacheService cache, ILogger<CachedProductsServiceClient> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Dictionary<Guid, bool>> CheckProductsExistAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var keys = ids.Select(id => CreateKey(id, ExistsKeyPrefix));
        var cachedRaw = await _cache.GetManyAsync<bool?>(keys, ct);
        var cached = cachedRaw.ToDictionary(
            kvp => ExtractIdFromKey(kvp.Key, ExistsKeyPrefix),
            kvp => kvp.Value);

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

        _logger.LogInformation("Cache miss for {Count} product exists checks", uncachedIds.Count);

        var freshResults = await _inner.CheckProductsExistAsync(uncachedIds, ct);

        await _cache.SetManyAsync(
            freshResults.ToDictionary(
                kvp => CreateKey(kvp.Key, ExistsKeyPrefix),
                kvp => kvp.Value),
            ExistsTtl, ct);

        var result = cached.ToDictionary(
            kvp => kvp.Key,
            kvp =>
            {
                if (kvp.Value is not null)
                    return (bool)kvp.Value;
                if (freshResults.TryGetValue(kvp.Key, out var value))
                    return value;

                _logger.LogWarning("Product {ProductId} missing in fresh results for existence check", kvp.Key);

                return default; // treat as non-existent if missing in both cache and fresh results
            });

        return result;
    }

    public async Task<List<ProductDto>> GetProductsInfoAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        var keys = productIds.Select(id => CreateKey(id, InfoKeyPrefix));
        var cachedRaw = await _cache.GetManyAsync<ProductDto>(keys, ct);
        var cached = cachedRaw.ToDictionary(
            kvp => ExtractIdFromKey(kvp.Key, InfoKeyPrefix),
            kvp => kvp.Value);

        var uncachedIds = cached
            .Where(kvp => kvp.Value is null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (!uncachedIds.Any())
        {
            _logger.LogDebug("All {Count} product infos loaded from cache", cached.Count);
            return cached.Values.OfType<ProductDto>().ToList();
        }

        _logger.LogInformation("Cache miss for {Count} product infos", uncachedIds.Count);

        var freshResults = await _inner.GetProductsInfoAsync(uncachedIds, ct);
        var freshDict = freshResults
            .ToDictionary(p => p.Id, p => p);

        await _cache.SetManyAsync(
            freshDict.ToDictionary(
                kvp => CreateKey(kvp.Key, InfoKeyPrefix),
                kvp => kvp.Value),
            InfoTtl, ct);

        var result = cached.Select(kvp =>
        {
            if (kvp.Value is not null)
                return kvp.Value;

            if (freshDict.TryGetValue(kvp.Key, out var value))
                return value;

            _logger.LogWarning("Product {ProductId} missing in fresh results for product info", kvp.Key);
            return CreateFallback(kvp.Key);
        }).ToList();

        return result;
    }

    private static Guid ExtractIdFromKey(string key, string prefix)
        => Guid.Parse(key.AsSpan(prefix.Length));


    private static string CreateKey(Guid id, string prefix)
        => $"{prefix}{id}";

    private static ProductDto CreateFallback(Guid id)
        => FallbackProductTemplate with { Id = id };
}
