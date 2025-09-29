using eCommerce.OrdersService.Application.MappingProfiles;
using eCommerce.OrdersService.Application.Queries.GetOrders;
using eCommerce.OrdersService.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace eCommerce.OrdersService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(OrderDtoMappingProfile));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetOrdersQuery>());
        services.AddValidatorsFromAssemblyContaining<AddOrderDtoValidator>();
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en-US");

        return services;
    }
}
