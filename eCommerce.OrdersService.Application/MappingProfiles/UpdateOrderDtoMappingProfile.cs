using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.MappingProfiles;

public class UpdateOrderDtoMappingProfile: Profile
{
    public UpdateOrderDtoMappingProfile()
    {
        CreateMap<UpdateOrderDto, Order>()
            .ConstructUsing((src, ctx) => Order.Restore(
                src.OrderId,
                src.UserId,
                src.OrderDate,
                src.TotalBill,
                ctx.Mapper.Map<List<OrderItem>>(src.UpdateOrderItemDtos)));
    }
}
