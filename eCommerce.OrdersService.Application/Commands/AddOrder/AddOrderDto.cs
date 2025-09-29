namespace eCommerce.OrdersService.Application.Commands.AddOrder;

public record AddOrderDto(
    Guid UserId,
    DateTime OrderDate,
    List<AddOrderItemDto> AddOrderItemDtos)
{
    public AddOrderDto() : this(default, default, []) { }
}
