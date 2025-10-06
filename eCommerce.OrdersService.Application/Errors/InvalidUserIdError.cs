using FluentResults;

namespace eCommerce.OrdersService.Application.Errors;

public class InvalidUserIdError(Guid userId, string message) : Error(message)
{
    public Guid UserId { get; set; } = userId;

    public static InvalidUserIdError WithId(Guid userId)
        => new(userId, $"User with id '{userId}' does not exist.");
}
