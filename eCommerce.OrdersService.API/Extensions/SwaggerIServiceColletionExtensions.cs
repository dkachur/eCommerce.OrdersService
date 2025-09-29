namespace eCommerce.OrdersService.API.Extensions;

public static class SwaggerIServiceColletionExtensions
{
    public static IServiceCollection AddConfiguredSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();

        return services;
    }
}
