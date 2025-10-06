using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.API.Enrichers;

namespace eCommerce.OrdersService.API.Extensions;

/// <summary>
/// Provides extension methods for adding response enrichers to DI.
/// </summary>
public static class EnricherServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="OrderResponseEnricher"/> as scoped service to DI.
    /// </summary>
    /// <param name="services">The service collection to add response enricher to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddEnrichers(this IServiceCollection services)
    {
        services.AddScoped<IResponseEnricher<OrderResponse>, OrderResponseEnricher>();
        services.AddScoped<IResponseEnricher<IEnumerable<OrderResponse>>, OrderResponseEnricher>();

        return services;
    }
}
