using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;

namespace eCommerce.OrdersService.Infrastructure.Messaging.HostedServices;

public class RabbitMqConnectionHostedService : IHostedService
{
    private readonly IRabbitMqConnectionManager _connectionManager;

    public RabbitMqConnectionHostedService(IRabbitMqConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _connectionManager.InitializeAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _connectionManager.DisposeAsync();
    }
}

