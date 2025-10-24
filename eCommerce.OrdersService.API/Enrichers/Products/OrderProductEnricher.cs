using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;

namespace eCommerce.OrdersService.API.Enrichers.Products;

public class OrderProductEnricher : IOrderProductEnricher
{
    private readonly IProductsServiceClient _productsServiceClient;
    private readonly IMapper _mapper;

    public OrderProductEnricher(IProductsServiceClient productsServiceClient, IMapper mapper)
    {
        _productsServiceClient = productsServiceClient;
        _mapper = mapper;
    }

    public async Task<OrderResponse> EnrichAsync(OrderResponse response, CancellationToken ct = default)
    {
        var products = await GetProductsByIdsAsync(response.OrderItems.Select(i => i.ProductId), ct);

        EnrichAllOrderItems(response.OrderItems, products);

        return response;
    }

    public async Task<IEnumerable<OrderResponse>> EnrichAsync(IEnumerable<OrderResponse> response, CancellationToken ct = default)
    {
        var productIds = response.SelectMany(r => r.OrderItems).Select(i => i.ProductId);
        var products = await GetProductsByIdsAsync(productIds, ct);

        foreach (var order in response)
        {
            EnrichAllOrderItems(order.OrderItems, products);
        }

        return response;
    }

    #region Helpers

    private async Task<Dictionary<Guid, ProductDto>> GetProductsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var products = await _productsServiceClient.GetProductsInfoAsync(ids.Distinct(), ct);
        return products
            .ToDictionary(p => p.Id, p => p);
    }

    private void EnrichAllOrderItems(List<OrderItemResponse> orderItems, Dictionary<Guid, ProductDto> products)
    {
        for (int i = 0; i < orderItems.Count; i++)
        {
            if (products.TryGetValue(orderItems[i].ProductId, out var product))
                orderItems[i] = orderItems[i] with 
                {
                    Category = product.Category,
                    Name = product.Name,
                };
        }
    }

    #endregion
}
