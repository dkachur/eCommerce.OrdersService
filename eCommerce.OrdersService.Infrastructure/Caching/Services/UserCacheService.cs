using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Infrastructure.Caching.Interfaces;
using eCommerce.OrdersService.Infrastructure.Caching.Keys;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.Caching.Services;

public class UserCacheService : IUserCacheService
{
    private readonly ICacheService _cache;
    private readonly ILogger<UserCacheService> _logger;

    private static readonly TimeSpan UserInfoTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan UserExistsTtl = TimeSpan.FromMinutes(1);

    public UserCacheService(ICacheService cacheService, ILogger<UserCacheService> logger)
    {
        _cache = cacheService;
        _logger = logger;
    }

    public async Task CacheUserInfoAsync(UserDto user, CancellationToken ct = default)
    {
        await _cache.SetAsync(UserCacheKeys.CreateInfoKey(user.UserId), user, UserInfoTtl, ct).ConfigureAwait(false);
        _logger.LogDebug("Cached user info for {UserId}", user.UserId);
    }
    
    public async Task CacheUserExistsAsync(Guid userId, bool exists, CancellationToken ct = default)
    {
        await _cache.SetAsync(UserCacheKeys.CreateExistsKey(userId), exists, UserExistsTtl, ct).ConfigureAwait(false);
        _logger.LogDebug("Cached user exists flag for {UserId}: {Exists}", userId, exists);
    }

    public async Task CacheUserInfosAsync(IEnumerable<UserDto> users, CancellationToken ct = default)
    {
        var userList = users.ToList();
        await _cache.SetManyAsync(
            userList.ToDictionary(
                user => UserCacheKeys.CreateInfoKey(user.UserId),
                user => user),
            UserInfoTtl, ct).ConfigureAwait(false);

        _logger.LogDebug("Cached {Count} user infos", userList.Count);
    }

    public Task<UserDto?> GetUserInfoAsync(Guid userId, CancellationToken ct = default)
        => _cache.GetAsync<UserDto>(UserCacheKeys.CreateInfoKey(userId), ct);

    public Task<bool?> GetUserExistsAsync(Guid userId, CancellationToken ct = default)
        => _cache.GetAsync<bool?>(UserCacheKeys.CreateExistsKey(userId), ct);

    public async Task<Dictionary<Guid, UserDto?>> GetUserInfosAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        var keys = userIds.Select(id => UserCacheKeys.CreateInfoKey(id));
        var cached = await _cache.GetManyAsync<UserDto>(keys, ct).ConfigureAwait(false);

        return cached.ToDictionary(
            kvp => UserCacheKeys.ExtractIdFromInfoKey(kvp.Key),
            kvp => kvp.Value);
    }
}
