using eCommerce.OrdersService.Infrastructure.Messaging.DTO;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;

namespace eCommerce.OrdersService.Infrastructure.Messaging.HostedServices;
public class RabbitMqConsumersHostedService : IHostedService
{
    private readonly IMessageConsumer<ProductNameUpdatedMessage> _productNameUpdatedConsumer;

    public RabbitMqConsumersHostedService(
        IMessageConsumer<ProductNameUpdatedMessage> productNameUpdatedConsumer)
    {
        _productNameUpdatedConsumer = productNameUpdatedConsumer;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        await _productNameUpdatedConsumer.StartConsumingAsync(ct);
    }

    public async Task StopAsync(CancellationToken ct)
    {
        await _productNameUpdatedConsumer.DisposeAsync();
    }
}
