using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.Commands.UpdateOrder;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class UpdateOrderRequestMappingProfile : Profile
{
    public UpdateOrderRequestMappingProfile()
    {
        CreateMap<UpdateOrderRequest, UpdateOrderDto>()
            .ForMember(dest => dest.UpdateOrderItemDtos, opt => opt.MapFrom(src => src.OrderItems));
    }
}
