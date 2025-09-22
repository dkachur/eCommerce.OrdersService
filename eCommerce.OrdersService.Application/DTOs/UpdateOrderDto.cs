namespace eCommerce.OrdersService.Application.DTOs;

public record UpdateOrderDto(
    Guid OrderId,
    Guid UserId,
    DateTime OrderDate,
    decimal TotalBill,
    List<UpdateOrderItemDto> UpdateOrderItemDtos);
