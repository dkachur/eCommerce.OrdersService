using eCommerce.OrdersService.Application.DTOs;

namespace eCommerce.OrdersService.Application.ServiceContracts;

public interface IProductsServiceClient
{
    Task<Dictionary<Guid, bool>> CheckProductsExistAsync(IEnumerable<Guid> ids, CancellationToken ct = default);

    Task<List<ProductDto>> GetProductInfosAsync(IEnumerable<Guid> productIds, CancellationToken ct = default);
}
