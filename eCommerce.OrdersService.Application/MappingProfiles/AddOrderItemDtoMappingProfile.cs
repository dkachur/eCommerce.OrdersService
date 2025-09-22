using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.MappingProfiles;

public class AddOrderItemDtoMappingProfile: Profile
{
    public AddOrderItemDtoMappingProfile()
    {
        CreateMap<AddOrderItemDto, OrderItem>()
            .ConstructUsing(src => OrderItem.New(
                src.ProductId,
                src.UnitPrice,
                src.Quantity));
    }
}
