using eCommerce.OrdersService.Infrastructure.MappingProfiles;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.OrdersService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(OrderItemMappingProfile).Assembly);

        return services;
    }
}
