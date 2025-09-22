using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.MappingProfiles;

public class UpdateOrderDtoItemMappingProfile: Profile
{
    public UpdateOrderDtoItemMappingProfile()
    {
        CreateMap<UpdateOrderItemDto, OrderItem>()
            .ConstructUsing(src => OrderItem.New(
                src.ProductId,
                src.UnitPrice,
                src.Quantity));
    }
}
