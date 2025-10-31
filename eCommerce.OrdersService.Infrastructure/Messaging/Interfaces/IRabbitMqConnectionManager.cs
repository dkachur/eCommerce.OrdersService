using RabbitMQ.Client;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;

public interface IRabbitMqConnectionManager : IAsyncDisposable
{
    IConnection Connection { get; }
    Task InitializeAsync(CancellationToken ct = default);
    Task<IChannel> CreateChannel();
}
