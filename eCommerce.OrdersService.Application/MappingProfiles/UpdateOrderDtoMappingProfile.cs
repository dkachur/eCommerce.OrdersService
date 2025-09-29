using AutoMapper;
using eCommerce.OrdersService.Application.Commands.UpdateOrder;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.MappingProfiles;

public class UpdateOrderDtoMappingProfile: Profile
{
    public UpdateOrderDtoMappingProfile()
    {
        CreateMap<UpdateOrderDto, Order>()
            .ConstructUsing((src, ctx) => Order.Update(
                src.OrderId,
                src.UserId,
                src.OrderDate,
                ctx.Mapper.Map<List<OrderItem>>(src.UpdateOrderItemDtos)));
    }
}
