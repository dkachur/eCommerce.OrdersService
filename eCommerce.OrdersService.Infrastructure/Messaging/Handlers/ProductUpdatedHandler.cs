using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Infrastructure.Caching.Interfaces;
using eCommerce.OrdersService.Infrastructure.Messaging.DTOs;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.Infrastructure.Messaging.Handlers;

public class ProductUpdatedHandler : IMessageHandler<ProductUpdatedMessage>
{
    private readonly ILogger<ProductUpdatedHandler> _logger;
    private readonly IProductCacheService _cache;
    private readonly IMapper _mapper;

    public ProductUpdatedHandler(ILogger<ProductUpdatedHandler> logger, IProductCacheService cache, IMapper mapper)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task HandleAsync(ProductUpdatedMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation("Product with ID {ProductId} updated", message.Id);
        await _cache.CacheProductInfoAsync(_mapper.Map<ProductDto>(message), ct).ConfigureAwait(false);
    }
}
