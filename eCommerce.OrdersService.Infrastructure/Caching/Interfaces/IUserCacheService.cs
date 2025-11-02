using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.Infrastructure.Caching.Interfaces;

public interface IUserCacheService
{
    Task CacheUserInfoAsync(UserDto user, CancellationToken ct = default);
    Task CacheUserExistsAsync(Guid userId, bool exists, CancellationToken ct = default);

    Task CacheUserInfosAsync(IEnumerable<UserDto> users, CancellationToken ct = default);

    Task<UserDto?> GetUserInfoAsync(Guid userId, CancellationToken ct = default);
    Task<bool?> GetUserExistsAsync(Guid userId, CancellationToken ct = default);

    Task<Dictionary<Guid, UserDto?>> GetUserInfosAsync(IEnumerable<Guid> userIds, CancellationToken ct = default);
}
