using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Users;

public class CachedUsersServiceClient : IUsersServiceClient
{
    private readonly IUsersServiceClient _inner;
    private readonly ICacheService _cache;
    private readonly ILogger<CachedUsersServiceClient> _logger;

    private const string ExistsKeyPrefix = "user-exists:";
    private const string InfoKeyPrefix = "user-info:";
    private static readonly TimeSpan ExistsTtl = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan InfoTtl = TimeSpan.FromMinutes(5);

    public CachedUsersServiceClient(IUsersServiceClient inner, ICacheService cache, ILogger<CachedUsersServiceClient> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> CheckUserExistsAsync(Guid userId, CancellationToken ct = default)
    {
        var key = CreateKey(userId, ExistsKeyPrefix);
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached is not null)
        {
            _logger.LogInformation("User exists flag for {UserId} loaded from cache", userId);
            return cached.Value;
        }

        _logger.LogInformation("Cache miss for {UserId} user existence check", userId);

        var fresh = await _inner.CheckUserExistsAsync(userId, ct);
        await _cache.SetAsync(key, fresh, ExistsTtl, ct);
        return fresh;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        var key = CreateKey(userId, InfoKeyPrefix);
        var cached = await _cache.GetAsync<UserDto?>(key, ct);
        if (cached is not null)
        {
            _logger.LogDebug("User info for {UserId} loaded from cache", userId);
            return cached;
        }

        _logger.LogInformation("Cache miss for {UserId} user info", userId);

        var fresh = await _inner.GetUserAsync(userId, ct);
        await _cache.SetAsync(key, fresh, InfoTtl, ct);
        return fresh;
    }

    public async Task<List<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        var keys = userIds
            .Select(id => CreateKey(id, InfoKeyPrefix));

        var cachedRaw = await _cache.GetManyAsync<UserDto?>(keys, ct);
        var cached = cachedRaw.ToDictionary(
            kvp => ExtractIdFromKey(kvp.Key, InfoKeyPrefix),
            kvp => kvp.Value);

        var uncachedIds = cached
            .Where(kvp => kvp.Value is null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (!uncachedIds.Any())
        {
            _logger.LogDebug("All {Count} user infos loaded from cache", cached.Count);
            return cached.Values.OfType<UserDto>().ToList();
        }

        _logger.LogInformation("Cache miss for {Count} user infos", uncachedIds.Count);

        var freshResults = await _inner.GetUsersByIdsAsync(uncachedIds, ct);
        var freshDict = freshResults
            .ToDictionary(user => user.UserId, user => user);

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

            _logger.LogWarning("User {UserId} not found in inner service results", kvp.Key);
            return null;
        }).OfType<UserDto>().ToList();

        return result;
    }

    private static string CreateKey(Guid id, string prefix)
        => $"{prefix}{id}";

    private static Guid ExtractIdFromKey(string key, string prefix)
        => Guid.Parse(key.AsSpan(prefix.Length));
}
