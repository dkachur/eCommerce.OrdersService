using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerce.OrdersService.Infrastructure.Persistence.Mongo;

public class OrderItemDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }

    [BsonRepresentation(BsonType.Double)]
    public decimal UnitPrice { get; set; }

    [BsonRepresentation(BsonType.Int32)]
    public int Quantity { get; set; }

    [BsonRepresentation(BsonType.Double)]
    public decimal TotalPrice { get; set; }
}