namespace eCommerce.OrdersService.Application.DTOs;

public record OrderDto(
    Guid OrderId, 
    Guid UserId, 
    DateTime OrderDate,
    decimal TotalBill,
    List<OrderItemDto> OrderItemDtos);