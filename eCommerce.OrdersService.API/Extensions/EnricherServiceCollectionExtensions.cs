using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.API.Enrichers;
using eCommerce.OrdersService.API.Enrichers.Products;
using eCommerce.OrdersService.API.Enrichers.Users;

namespace eCommerce.OrdersService.API.Extensions;

/// <summary>
/// Provides extension methods for adding response enrichers to DI.
/// </summary>
public static class EnricherServiceCollectionExtensions
{
    /// <summary>
    /// Adds enrichers for <see cref="OrderResponse"/> as scoped services to DI.
    /// </summary>
    /// <param name="services">The service collection to add response enrichers to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddEnrichers(this IServiceCollection services)
    {
        services.AddScoped<IOrderProductEnricher, OrderProductEnricher>();
        services.AddScoped<IOrderUserEnricher, OrderUserEnricher>();
        services.AddScoped<IOrderResponseEnricher, CompositeOrderEnricher>();

        return services;
    }
}
