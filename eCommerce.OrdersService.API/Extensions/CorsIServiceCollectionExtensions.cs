namespace eCommerce.OrdersService.API.Extensions;

/// <summary>
/// Provides extension methods for configuring Cross-Origin Resource Sharing (CORS).
/// </summary>
public static class CorsIServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the default CORS policy using the <c>AllowedOrigins</c> configuration section.
    /// </summary>
    /// <param name="services">The service collection to add CORS configuration to.</param>
    /// <param name="config">The application configuration containing the allowed origins.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
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
