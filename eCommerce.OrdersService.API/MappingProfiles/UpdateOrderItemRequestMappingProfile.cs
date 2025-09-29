using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.Commands.UpdateOrder;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class UpdateOrderItemRequestMappingProfile : Profile
{
    public UpdateOrderItemRequestMappingProfile()
    {
        CreateMap<UpdateOrderItemRequest, UpdateOrderItemDto>();
    }
}
