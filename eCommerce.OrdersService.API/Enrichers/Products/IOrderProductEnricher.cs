using eCommerce.OrdersService.API.DTOs;

namespace eCommerce.OrdersService.API.Enrichers.Products;

public interface IOrderProductEnricher : IResponseEnricher<OrderResponse>, IResponseEnricher<IEnumerable<OrderResponse>> { }
