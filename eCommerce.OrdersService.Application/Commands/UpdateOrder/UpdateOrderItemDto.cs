namespace eCommerce.OrdersService.Application.Commands.UpdateOrder;

public record UpdateOrderItemDto(
    Guid ProductId,
    decimal UnitPrice,
    int Quantity);
