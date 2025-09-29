namespace eCommerce.OrdersService.API.DTOs;

public record AddOrderRequest(
    Guid UserId,
    DateTime OrderDate,
    List<AddOrderItemRequest> OrderItems);
