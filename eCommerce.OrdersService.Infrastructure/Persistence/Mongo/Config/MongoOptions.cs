namespace eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;
public class MongoOptions
{
    public string User { get; set; } = string.Empty;
    public string Pass { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public CollectionOptions Collections { get; set; } = new();
}

