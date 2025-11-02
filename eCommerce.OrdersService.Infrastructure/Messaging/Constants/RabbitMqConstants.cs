namespace eCommerce.OrdersService.Infrastructure.Messaging.Constants;

public static class RabbitMqConstants
{
    public const string ProductUpdatedQueue = "orders.product.updated.queue";
    public const string ProductDeletedQueue = "orders.product.deleted.queue";
}
