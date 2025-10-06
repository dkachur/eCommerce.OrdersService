using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.API.Enrichers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eCommerce.OrdersService.API.Filters;

public class OrderResponseEnrichmentFilter : IAsyncResultFilter
{
    private readonly IResponseEnricher<OrderResponse> _singleOrderEnricher;
    private readonly IResponseEnricher<IEnumerable<OrderResponse>> _multipleOrdersEnricher;
    private readonly ILogger<OrderResponseEnrichmentFilter> _logger;

    public OrderResponseEnrichmentFilter(
        IResponseEnricher<OrderResponse> singleOrderEnricher,
        IResponseEnricher<IEnumerable<OrderResponse>> multipleOrdersEnricher,
        ILogger<OrderResponseEnrichmentFilter> logger)
    {
        _singleOrderEnricher = singleOrderEnricher;
        _multipleOrdersEnricher = multipleOrdersEnricher;
        _logger = logger;
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
                OrderResponse order => await _singleOrderEnricher.EnrichAsync(order, ct),
                IEnumerable<OrderResponse> orders => await _multipleOrdersEnricher.EnrichAsync(orders, ct),
                _ => objRes.Value
            };
        }

        await next();
    }
}
