namespace eCommerce.OrdersService.API.DTOs;

public record UpdateOrderItemRequest(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity);