using AutoMapper;
using eCommerce.OrdersService.Domain.Entities;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo;

namespace eCommerce.OrdersService.Infrastructure.MappingProfiles;

public class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDocument>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<OrderItemDocument, OrderItem>()
            .ConstructUsing(src => OrderItem.Restore(src.ProductId, src.UnitPrice, src.Quantity, src.TotalPrice));
    }
}
