using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.Commands.AddOrder;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class AddOrderRequestMappingProfile : Profile
{
    public AddOrderRequestMappingProfile()
    {
        CreateMap<AddOrderRequest, AddOrderDto>()
            .ForMember(dest => dest.AddOrderItemDtos, opt => opt.MapFrom(src => src.OrderItems));
    }
}
