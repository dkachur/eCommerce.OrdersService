namespace eCommerce.OrdersService.API.Extensions;

public static class AutomapperIServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredAutomapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(Program));

        return services;
    }
}
