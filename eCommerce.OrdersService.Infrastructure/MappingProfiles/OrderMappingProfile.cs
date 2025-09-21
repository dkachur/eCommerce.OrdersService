using AutoMapper;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Infrastructure.MappingProfiles;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDocument>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(o => o.OrderItems));

        CreateMap<OrderDocument, Order>()
            .ConstructUsing(src => Order.Restore(
                src.OrderId,
                src.UserId,
                src.OrderDate,
                src.TotalBill,
                src.OrderItems.Select(item =>
                    OrderItem.Restore(item.ProductId, item.UnitPrice, item.Quantity, item.TotalPrice)).ToList()));
    }
}
