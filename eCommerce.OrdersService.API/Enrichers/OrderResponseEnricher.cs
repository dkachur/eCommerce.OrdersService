using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;

namespace eCommerce.OrdersService.API.Enrichers;

public class OrderResponseEnricher : IResponseEnricher<OrderResponse>, IResponseEnricher<IEnumerable<OrderResponse>>
{
    private readonly IProductsServiceClient _productsServiceClient;
    private readonly IMapper _mapper;

    public OrderResponseEnricher(IProductsServiceClient productsServiceClient, IMapper mapper)
    {
        _productsServiceClient = productsServiceClient;
        _mapper = mapper;
    }

    public async Task<OrderResponse> EnrichAsync(OrderResponse response, CancellationToken ct = default)
    {
        var products = await GetProductsByIds(response.OrderItems.Select(i => i.ProductId), ct);

        EnrichAllOrderItems(response.OrderItems, products);

        return response;
    }

    public async Task<IEnumerable<OrderResponse>> EnrichAsync(IEnumerable<OrderResponse> response, CancellationToken ct = default)
    {
        var productIds = response.SelectMany(r => r.OrderItems).Select(i => i.ProductId);
        var products = await GetProductsByIds(productIds, ct);

        foreach (var order in response)
        {
            EnrichAllOrderItems(order.OrderItems, products);
        }

        return response;
    }

    #region Helpers

    private async Task<Dictionary<Guid, ProductDto>> GetProductsByIds(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var products = await _productsServiceClient.GetProductsInfoAsync(ids.Distinct(), ct);
        return products
            .ToDictionary(p => p.Id, p => p);
    }

    private void EnrichAllOrderItems(List<OrderItemResponse> orderItems, Dictionary<Guid, ProductDto> products)
    {
        for (int i = 0; i < orderItems.Count; i++)
        {
            var item = orderItems[i];
            if (products.TryGetValue(item.ProductId, out var product))
                orderItems[i] = _mapper.Map<ProductDto, OrderItemResponse>(product, item);
        }
    }

    #endregion
}
