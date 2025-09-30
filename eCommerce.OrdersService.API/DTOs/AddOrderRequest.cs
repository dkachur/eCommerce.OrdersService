namespace eCommerce.OrdersService.API.DTOs;

/// <summary>
/// Data transfer object for binding the request to add a new order.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="OrderDate">The date of the order.</param>
/// <param name="OrderItems">The collection of items in the order.</param>
public record AddOrderRequest(
    Guid UserId,
    DateTime OrderDate,
    List<AddOrderItemRequest> OrderItems);
