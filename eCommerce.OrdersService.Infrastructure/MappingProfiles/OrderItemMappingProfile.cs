using AutoMapper;
using eCommerce.OrdersService.Domain.Entities;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Documents;

namespace eCommerce.OrdersService.Infrastructure.MappingProfiles;

public class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDocument>()
            //.ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(o => o.UnitPrice.Value))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(o => o.Quantity.Value))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(o => o.TotalPrice.Value));

        CreateMap<OrderItemDocument, OrderItem>()
            .ConstructUsing(src => OrderItem.Restore(src.ProductId, src.UnitPrice, src.Quantity, src.TotalPrice));
    }
}
