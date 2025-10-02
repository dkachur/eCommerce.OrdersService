using FluentResults;

namespace eCommerce.OrdersService.Application.Errors;

public class InvalidUserIdError(string message) : Error(message)
{
    public static InvalidUserIdError WithId(Guid userId)
        => new($"User with id '{userId}' does not exist.");
}
