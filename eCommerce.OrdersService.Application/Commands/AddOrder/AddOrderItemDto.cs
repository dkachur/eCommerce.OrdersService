namespace eCommerce.OrdersService.Application.Commands.AddOrder;

public record AddOrderItemDto(
    Guid ProductId, 
    decimal UnitPrice, 
    int Quantity);
