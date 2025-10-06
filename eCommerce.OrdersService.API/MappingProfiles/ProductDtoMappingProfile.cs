using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class ProductDtoMappingProfile : Profile
{
    public ProductDtoMappingProfile()
    {
        CreateMap<ProductDto, OrderItemResponse>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForAllMembers(opt =>
            {
                if (opt.DestinationMember.Name != nameof(OrderItemResponse.Category)
                    && opt.DestinationMember.Name != nameof(OrderItemResponse.Name))
                {
                    opt.Ignore();
                }
            });
    }
}
