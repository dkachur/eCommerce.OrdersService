namespace eCommerce.OrdersService.API.Extensions;

public static class CorsIServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = config.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
                policyBuilder.WithOrigins(origins ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
        });

        return services;
    }
}
