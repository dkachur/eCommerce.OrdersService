namespace eCommerce.OrdersService.Application.DTOs;

public record AddOrderItemDto(
    Guid ProductId, 
    decimal UnitPrice, 
    int Quantity);
