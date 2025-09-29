namespace eCommerce.OrdersService.API.DTOs;

public record AddOrderItemRequest(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity);