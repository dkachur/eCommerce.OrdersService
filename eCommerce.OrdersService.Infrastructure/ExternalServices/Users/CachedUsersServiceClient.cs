using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.Caching.Interfaces;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Users;

public class CachedUsersServiceClient : IUsersServiceClient
{
    private readonly IUsersServiceClient _inner;
    private readonly IUserCacheService _cache;
    private readonly ILogger<CachedUsersServiceClient> _logger;

    public CachedUsersServiceClient(
        IUsersServiceClient inner,
        IUserCacheService cache,
        ILogger<CachedUsersServiceClient> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> CheckUserExistsAsync(Guid userId, CancellationToken ct = default)
    {
        var cached = await _cache.GetUserExistsAsync(userId, ct).ConfigureAwait(false);
        if (cached is not null)
        {
            _logger.LogInformation("User exists flag for {UserId} loaded from cache", userId);
            return cached.Value;
        }

        _logger.LogInformation("Cache miss for {UserId} user existence check", userId);

        var fresh = await _inner.CheckUserExistsAsync(userId, ct).ConfigureAwait(false);
        await _cache.CacheUserExistsAsync(userId, fresh, ct).ConfigureAwait(false);
        return fresh;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        var cached = await _cache.GetUserInfoAsync(userId, ct).ConfigureAwait(false);
        if (cached is not null)
        {
            _logger.LogDebug("User info for {UserId} loaded from cache", userId);
            return cached;
        }

        _logger.LogInformation("Cache miss for {UserId} user info", userId);

        var fresh = await _inner.GetUserAsync(userId, ct).ConfigureAwait(false);

        if (fresh is not null)
            await _cache.CacheUserInfoAsync(fresh, ct).ConfigureAwait(false);

        return fresh;
    }

    public async Task<List<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        var cached = await _cache.GetUserInfosAsync(userIds, ct).ConfigureAwait(false);

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
        await _cache.CacheUserInfosAsync(freshResults, ct).ConfigureAwait(false);

        var freshDict = freshResults
            .ToDictionary(user => user.UserId, user => user);

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
}
