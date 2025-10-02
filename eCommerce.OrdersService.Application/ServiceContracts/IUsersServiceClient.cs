using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.Application.ServiceContracts;

public interface IUsersServiceClient
{
    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken ct = default);
}
