using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class OrderResponseMappingProfile : Profile
{
    public OrderResponseMappingProfile()
    {
        CreateMap<OrderDto, OrderResponse>();
    }
}
