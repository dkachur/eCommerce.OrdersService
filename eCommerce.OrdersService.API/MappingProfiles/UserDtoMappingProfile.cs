using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.API.MappingProfiles;

public class UserDtoMappingProfile : Profile
{
    public UserDtoMappingProfile()
    {
        CreateMap<UserDto, OrderResponse>()
            .ForMember(dest => dest.UserPersonName, opt => opt.MapFrom(src => src.PersonName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForAllMembers(opt =>
            {
                if (opt.DestinationMember.Name != nameof(OrderResponse.UserPersonName)
                    && opt.DestinationMember.Name != nameof(OrderResponse.Email))
                {
                    opt.Ignore();
                }
            });
    }
}
