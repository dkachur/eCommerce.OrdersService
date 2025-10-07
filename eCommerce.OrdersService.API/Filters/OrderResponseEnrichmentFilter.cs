using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.API.Enrichers;
using eCommerce.OrdersService.API.Enrichers.Products;
using eCommerce.OrdersService.API.Enrichers.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eCommerce.OrdersService.API.Filters;

public class OrderResponseEnrichmentFilter : IAsyncResultFilter
{
    private readonly IOrderResponseEnricher _enricher;
    private readonly ILogger<OrderResponseEnrichmentFilter> _logger;

    public OrderResponseEnrichmentFilter(
        ILogger<OrderResponseEnrichmentFilter> logger,
        IOrderResponseEnricher enricher)
    {
        _logger = logger;
        _enricher = enricher;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var objRes = context.Result as ObjectResult;

        if (objRes is not null && objRes.Value is not null)
        {
            _logger.LogDebug("Enriching {Type}", objRes.Value.GetType().Name);

            var ct = context.HttpContext.RequestAborted;

            objRes.Value = objRes.Value switch
            {
                OrderResponse order => await _enricher.EnrichAsync(order, ct),
                IEnumerable<OrderResponse> orders => await _enricher.EnrichAsync(orders, ct),
                _ => objRes.Value
            };
        }

        await next();
    }
}
