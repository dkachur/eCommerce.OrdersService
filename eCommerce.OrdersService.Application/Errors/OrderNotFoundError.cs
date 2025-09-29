using FluentResults;

namespace eCommerce.OrdersService.Application.Errors;

public class OrderNotFoundError(string message) : Error(message)
{
    public static OrderNotFoundError WithId(Guid orderId)
        => new($"Order with ID '{orderId}' does not exist.");
}
