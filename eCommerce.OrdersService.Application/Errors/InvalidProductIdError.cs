using FluentResults;

namespace eCommerce.OrdersService.Application.Errors;

public class InvalidProductIdError(Guid productId, string message) : Error(message)
{
    public Guid ProductId { get; set; } = productId;

    public static InvalidProductIdError WithId(Guid productId)
        => new(productId, $"Product with id '{productId}' does not exist.");
}
