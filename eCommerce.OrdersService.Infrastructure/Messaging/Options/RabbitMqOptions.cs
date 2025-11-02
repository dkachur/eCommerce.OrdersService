namespace eCommerce.OrdersService.Infrastructure.Messaging.Options;

public class RabbitMqOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ProductsExchange { get; set; } = string.Empty;
    public string ProductUpdatedRoutingKey { get; set; } = string.Empty;
    public string ProductDeletedRoutingKey { get; set; } = string.Empty;
}
