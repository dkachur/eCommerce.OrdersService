using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace eCommerce.OrdersService.Infrastructure.Extensions;

public static class MongoDbExtensions
{
    public static async Task<IApplicationBuilder> EnsureMongoIndexesAsync(this IApplicationBuilder app, CancellationToken ct = default)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        var collections = scope.ServiceProvider.GetRequiredService<IOptions<MongoOptions>>().Value.Collections;

        await MongoIndexesInitializer.EnsureIndexesAsync(db, collections.Orders, ct);

        return app;
    }
}
