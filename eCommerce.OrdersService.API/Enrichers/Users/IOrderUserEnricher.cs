using eCommerce.OrdersService.API.DTOs;

namespace eCommerce.OrdersService.API.Enrichers.Users;

public interface IOrderUserEnricher : IResponseEnricher<OrderResponse>, IResponseEnricher<IEnumerable<OrderResponse>> { }
