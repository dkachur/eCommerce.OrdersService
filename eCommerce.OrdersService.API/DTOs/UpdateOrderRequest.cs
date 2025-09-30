namespace eCommerce.OrdersService.API.DTOs;

/// <summary>
/// Data transfer object for binding the request to update an existing order.
/// </summary>
/// <param name="OrderId">The unique identifier of the order to update.</param>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="OrderDate">The date of the order.</param>
/// <param name="OrderItems">The collection of items in the order.</param>
public record UpdateOrderRequest(
    Guid OrderId,
    Guid UserId,
    DateTime OrderDate,
    List<UpdateOrderItemRequest> OrderItems);