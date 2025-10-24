using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.Application.ServiceContracts;

public interface IUsersServiceClient
{
    Task<bool> CheckUserExistsAsync(Guid userId, CancellationToken ct = default);

    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken ct = default);

    Task<List<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default);
}
