using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;

namespace eCommerce.OrdersService.API.Enrichers.Users;

public class OrderUserEnricher : IOrderUserEnricher
{
    private readonly IUsersServiceClient _usersServiceClient;

    public OrderUserEnricher(IUsersServiceClient usersServiceClient)
    {
        _usersServiceClient = usersServiceClient;
    }

    public async Task<OrderResponse> EnrichAsync(OrderResponse response, CancellationToken ct = default)
    {
        var user = await _usersServiceClient.GetUserAsync(response.UserId, ct);

        if (user is not null)
            response = EnrichWithUserInfo(response, user);

        return response;
    }

    public async Task<IEnumerable<OrderResponse>> EnrichAsync(IEnumerable<OrderResponse> response, CancellationToken ct = default)
    {
        List<OrderResponse> orders = response.ToList();
        var userIds = response.Select(o => o.UserId).ToHashSet();
        var users = await GetUsersByIds(userIds, ct);

        for (int i = 0; i < orders.Count; i++)
        {
            if (users.TryGetValue(orders[i].UserId, out var user))
                orders[i] = EnrichWithUserInfo(orders[i], user);
        }

        return orders;
    }

    #region Helpers

    private OrderResponse EnrichWithUserInfo(OrderResponse order, UserDto user)
    {
        return order with
        {
            Email = user.Email,
            UserPersonName = user.PersonName,
        };
    }

    private async Task<Dictionary<Guid, UserDto>> GetUsersByIds(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var users = await _usersServiceClient.GetUsersByIdsAsync(ids, ct);
        return users
            .ToDictionary(u => u.UserId, u => u);
    }

    #endregion
}
