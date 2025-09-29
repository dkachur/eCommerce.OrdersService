using AutoMapper;
using eCommerce.OrdersService.Domain.Entities;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Documents;

namespace eCommerce.OrdersService.Infrastructure.MappingProfiles;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDocument>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TotalBill, opt => opt.MapFrom(o => o.TotalBill.Value))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(o => o.OrderItems));

        CreateMap<OrderDocument, Order>()
            .ConstructUsing((src, ctx) => 
            {
                var items = ctx.Mapper.Map<List<OrderItem>>(src.OrderItems);
                return Order.Restore(
                    src.OrderId,
                    src.UserId,
                    src.OrderDate,
                    src.TotalBill,
                    items);
            })
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
    }
}
