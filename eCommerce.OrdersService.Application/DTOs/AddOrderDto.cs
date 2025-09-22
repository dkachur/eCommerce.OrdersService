namespace eCommerce.OrdersService.Application.DTOs;

public record AddOrderDto(
    Guid UserId,
    DateTime OrderDate,
    List<AddOrderItemDto> AddOrderItemDtos);
