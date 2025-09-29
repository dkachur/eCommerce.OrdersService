namespace eCommerce.OrdersService.API.DTOs;

public record OrderItemResponse(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice);