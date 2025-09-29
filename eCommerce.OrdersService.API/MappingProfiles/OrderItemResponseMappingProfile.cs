using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class OrderItemResponseMappingProfile : Profile
{
    public OrderItemResponseMappingProfile()
    {
        CreateMap<OrderItemDto, OrderItemResponse>();
    }
}
