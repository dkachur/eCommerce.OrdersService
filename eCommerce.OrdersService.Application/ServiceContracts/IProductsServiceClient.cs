namespace eCommerce.OrdersService.Application.ServiceContracts;

public interface IProductsServiceClient
{
    Task<Dictionary<Guid, bool>> CheckProductsExistAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}
