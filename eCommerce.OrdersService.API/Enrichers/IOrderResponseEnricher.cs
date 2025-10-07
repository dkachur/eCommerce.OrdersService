using eCommerce.OrdersService.API.DTOs;

namespace eCommerce.OrdersService.API.Enrichers;

public interface IOrderResponseEnricher : IResponseEnricher<OrderResponse>, IResponseEnricher<IEnumerable<OrderResponse>> { }
