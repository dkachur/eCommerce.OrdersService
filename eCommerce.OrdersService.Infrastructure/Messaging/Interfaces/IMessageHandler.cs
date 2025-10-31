namespace eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;

public interface IMessageHandler<T>
{
    public Task HandleAsync(T message, CancellationToken ct = default);
}
