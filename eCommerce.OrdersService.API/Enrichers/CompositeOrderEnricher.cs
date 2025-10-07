using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.API.Enrichers.Products;
using eCommerce.OrdersService.API.Enrichers.Users;

namespace eCommerce.OrdersService.API.Enrichers;

public class CompositeOrderEnricher : IOrderResponseEnricher
{
    private readonly IOrderProductEnricher _productEnricher;
    private readonly IOrderUserEnricher _userEnricher;

    public CompositeOrderEnricher(IOrderProductEnricher productEnricher, IOrderUserEnricher userEnricher)
    {
        _productEnricher = productEnricher;
        _userEnricher = userEnricher;
    }

    public async Task<OrderResponse> EnrichAsync(OrderResponse response, CancellationToken ct = default)
    {
        response = await _productEnricher.EnrichAsync(response, ct);
        response = await _userEnricher.EnrichAsync(response, ct);

        return response;
    }

    public async Task<IEnumerable<OrderResponse>> EnrichAsync(IEnumerable<OrderResponse> response, CancellationToken ct = default)
    {
        response = await _productEnricher.EnrichAsync(response, ct);
        response = await _userEnricher.EnrichAsync(response, ct);

        return response;
    }
}
