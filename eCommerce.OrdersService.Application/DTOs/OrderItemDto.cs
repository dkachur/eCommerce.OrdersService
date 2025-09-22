namespace eCommerce.OrdersService.Application.DTOs;

public record OrderItemDto(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice);
