namespace eCommerce.OrdersService.API.DTOs;

/// <summary>
/// Data transfer object that represents the response order item information.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="UnitPrice">The unit price.</param>
/// <param name="Quantity">The product quantity.</param>
/// <param name="TotalPrice">The total price for all products.</param>
/// <param name="Category">The category of the product.</param>
/// <param name="Name">The name of the product.</param>
public record OrderItemResponse(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice,
    string? Category,
    string? Name)
{
    public OrderItemResponse() : this(default, default, default, default, default, default) { }
}