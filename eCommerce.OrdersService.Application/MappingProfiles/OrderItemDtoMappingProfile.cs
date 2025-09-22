using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.MappingProfiles;

public class OrderItemDtoMappingProfile: Profile
{
    public OrderItemDtoMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(o => o.ProductId))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(o => o.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(o => o.Quantity))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(o => o.TotalPrice));
    }
}
