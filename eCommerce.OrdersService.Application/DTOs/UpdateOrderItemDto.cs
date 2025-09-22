namespace eCommerce.OrdersService.Application.DTOs;

public record UpdateOrderItemDto(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity);
