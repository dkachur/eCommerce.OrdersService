using eCommerce.OrdersService.Infrastructure.Messaging.DTOs;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;

namespace eCommerce.OrdersService.Infrastructure.Messaging.HostedServices;
public class RabbitMqConsumersHostedService : IHostedService
{
    private readonly IMessageConsumer<ProductUpdatedMessage> _productNameUpdatedConsumer;
    private readonly IMessageConsumer<ProductDeletedMessage> _productDeletedConsumer;

    public RabbitMqConsumersHostedService(
        IMessageConsumer<ProductUpdatedMessage> productNameUpdatedConsumer, 
        IMessageConsumer<ProductDeletedMessage> productDeletedConsumer)
    {
        _productNameUpdatedConsumer = productNameUpdatedConsumer;
        _productDeletedConsumer = productDeletedConsumer;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        await _productNameUpdatedConsumer.StartConsumingAsync(ct);
        await _productDeletedConsumer.StartConsumingAsync(ct);
    }

    public async Task StopAsync(CancellationToken ct)
    {
        await _productNameUpdatedConsumer.DisposeAsync();
        await _productDeletedConsumer.DisposeAsync();
    }
}
