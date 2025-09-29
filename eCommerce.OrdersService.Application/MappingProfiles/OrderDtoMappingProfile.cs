using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.MappingProfiles;

public class OrderDtoMappingProfile : Profile
{
    public OrderDtoMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(o => o.OrderId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(o => o.UserId))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(o => o.OrderDate))
            .ForMember(dest => dest.TotalBill, opt => opt.MapFrom(o => o.TotalBill.Value))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(o => o.OrderItems));
    }
}
