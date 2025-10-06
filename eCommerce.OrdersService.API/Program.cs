using eCommerce.OrdersService.API.Extensions;
using eCommerce.OrdersService.API.Filters;
using eCommerce.OrdersService.API.Middlewares;
using eCommerce.OrdersService.Application;
using eCommerce.OrdersService.Infrastructure;
using eCommerce.OrdersService.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
                .AddInfrastructure(builder.Configuration);

builder.Services.AddEnrichers();

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<OrderResponseEnrichmentFilter>();
});

builder.Services.AddConfiguredAutomapper()
                .AddConfiguredSwagger()
                .AddConfiguredCors(builder.Configuration);

var app = builder.Build();

await app.EnsureMongoIndexesAsync(app.Lifetime.ApplicationStopping);

app.UseExceptionHandlingMiddleware();
app.UseRouting();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
