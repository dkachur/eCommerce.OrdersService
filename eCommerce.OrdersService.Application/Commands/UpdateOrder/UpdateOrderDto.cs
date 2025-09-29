namespace eCommerce.OrdersService.Application.Commands.UpdateOrder;

public record UpdateOrderDto(
    Guid OrderId,
    Guid UserId,
    DateTime OrderDate,
    List<UpdateOrderItemDto> UpdateOrderItemDtos)
{
    public UpdateOrderDto() : this(default, default, default, []) { }
}
