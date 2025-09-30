namespace eCommerce.OrdersService.API.Extensions;

/// <summary>
/// Provides extension methods for configuring Swagger/OpenAPI.
/// </summary>
public static class SwaggerIServiceColletionExtensions
{
    /// <summary>
    /// Adds and configures Swagger generation for the API.
    /// </summary>
    /// <param name="services">The service collection to add Swagger services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddConfiguredSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt => opt.IncludeXmlComments("api.xml"));
        return services;
    }
}
