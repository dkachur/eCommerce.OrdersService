using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Documents;
using MongoDB.Driver;

namespace eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;

public static class MongoIndexesInitializer
{
    public static async Task EnsureIndexesAsync(IMongoDatabase database, string collectionName, CancellationToken ct = default)
    {
        var orders = database.GetCollection<OrderDocument>(collectionName);
        var indexModels = new List<CreateIndexModel<OrderDocument>>
        {
            new (
                Builders<OrderDocument>.IndexKeys.Ascending(d => d.OrderId), 
                new CreateIndexOptions() { Unique = true }
            ),

            new (
                Builders<OrderDocument>.IndexKeys.Ascending(d => d.UserId)
            ),

            new (
                Builders<OrderDocument>.IndexKeys.Ascending("OrderItems.ProductId")
            )
        };

        await orders.Indexes.CreateManyAsync(indexModels, ct);
    }
}
