namespace eCommerce.OrdersService.API.DTOs;

/// <summary>
/// Data transfer object for binding the request to add a new order item.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="UnitPrice">The unit price.</param>
/// <param name="Quantity">The product quantity.</param>
public record AddOrderItemRequest(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity);