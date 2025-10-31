namespace eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;

public interface IMessageConsumer<T> : IAsyncDisposable
{
    Task StartConsumingAsync(CancellationToken ct = default);
}
