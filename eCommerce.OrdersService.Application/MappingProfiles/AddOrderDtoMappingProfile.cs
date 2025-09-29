using AutoMapper;
using eCommerce.OrdersService.Application.Commands.AddOrder;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.MappingProfiles;

public class AddOrderDtoMappingProfile : Profile
{
    public AddOrderDtoMappingProfile()
    {
        CreateMap<AddOrderDto, Order>()
            .ConstructUsing((src, ctx) => Order.New(
                src.UserId,
                src.OrderDate,
                ctx.Mapper.Map<List<OrderItem>>(src.AddOrderItemDtos)));
    }
}
