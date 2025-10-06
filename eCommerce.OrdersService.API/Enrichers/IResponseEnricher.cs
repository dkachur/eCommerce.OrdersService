namespace eCommerce.OrdersService.API.Enrichers;

public interface IResponseEnricher<TResponse>
{
    Task<TResponse> EnrichAsync(TResponse response, CancellationToken ct = default);
}
