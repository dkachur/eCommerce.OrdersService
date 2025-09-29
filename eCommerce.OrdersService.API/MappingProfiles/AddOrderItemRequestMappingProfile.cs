using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.Commands.AddOrder;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class AddOrderItemRequestMappingProfile : Profile
{
    public AddOrderItemRequestMappingProfile()
    {
        CreateMap<AddOrderItemRequest, AddOrderItemDto>();
    }
}
