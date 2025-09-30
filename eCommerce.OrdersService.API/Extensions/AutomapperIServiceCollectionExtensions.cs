namespace eCommerce.OrdersService.API.Extensions;

/// <summary>
/// Provides extension methods for registering AutoMapper in <see cref="IServiceCollection"/>.
/// </summary>
public static class AutomapperIServiceCollectionExtensions
{
    /// <summary>
    /// Registers AutoMapper and all mapping profiles from the API project.
    /// </summary>
    /// <param name="services">The service collection to add AutoMapper to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddConfiguredAutomapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(Program));

        return services;
    }
}
