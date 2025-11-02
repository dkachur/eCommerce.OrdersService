using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Infrastructure.Messaging.DTOs;

namespace eCommerce.OrdersService.Infrastructure.MappingProfiles;

public class ProductUpdatedMessageMappingProfile : Profile
{
    public ProductUpdatedMessageMappingProfile()
    {
        CreateMap<ProductUpdatedMessage, ProductDto>();
    }
}
