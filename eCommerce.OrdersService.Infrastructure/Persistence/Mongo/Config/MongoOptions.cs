namespace eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;
public class MongoOptions
{
    public string Database { get; set; } = string.Empty;
    public CollectionOptions Collections { get; set; } = new();
}

