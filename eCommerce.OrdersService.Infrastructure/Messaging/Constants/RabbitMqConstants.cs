namespace eCommerce.OrdersService.Infrastructure.Messaging.Constants;

public static class RabbitMqConstants
{
    public const string ProductNameUpdatedQueue = "orders.product.name.updated.queue";
    public const string ProductDeletedQueue = "orders.product.deleted.queue";
}
