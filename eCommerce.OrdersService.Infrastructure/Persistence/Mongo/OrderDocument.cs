using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerce.OrdersService.Infrastructure.Persistence.Mongo;

public class OrderDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid OrderId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime OrderDate { get; set; }

    [BsonRepresentation(BsonType.Double)]
    public decimal TotalBill { get; set; }

    public List<OrderItemDocument> OrderItems { get; set; } = [];
}
