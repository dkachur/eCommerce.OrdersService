namespace eCommerce.OrdersService.API.DTOs;

public record UpdateOrderRequest(
    Guid OrderId,
    Guid UserId,
    DateTime OrderDate,
    List<UpdateOrderItemRequest> OrderItems);