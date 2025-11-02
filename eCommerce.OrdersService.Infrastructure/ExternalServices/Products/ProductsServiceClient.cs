using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Products.DTOs;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net;
using System.Net.Http.Json;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Products;

public class ProductsServiceClient : IProductsServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsServiceClient> _logger;

    private const string ProductsEndpoint = "/gateway/products";
    private const string UnavailableText = "Temporarily unavailable";
    private static readonly ProductDto FallbackProductTemplate = new(
        Guid.Empty,
        UnavailableText,
        UnavailableText,
        default,
        default);

    public ProductsServiceClient(HttpClient httpClient, ILogger<ProductsServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Dictionary<Guid, bool>> CheckProductsExistAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await ExecuteSafeAsync(
            operation: async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{ProductsEndpoint}/exists", ids, ct);

                EnsureSuccessResponse(response);

                var content = await response.Content.ReadFromJsonAsync<IEnumerable<ProductExistenceResponse>>(ct);
                if (content is null)
                {
                    _logger.LogError("Products Service returned an empty or invalid response.");
                    throw new InvalidDataException("Invalid response from Products Service.");
                }

                var dictionary = content
                    .GroupBy(p => p.ProductId)
                    .Select(g => g.First())
                    .ToDictionary(p => p.ProductId, p => p.Exists);

                return dictionary;
            },
            onFailure: () => throw CreateUnavailableException("Unable to verify product existence."));
    }

    public async Task<List<ProductDto>> GetProductInfosAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        return await ExecuteSafeAsync(
            operation: async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{ProductsEndpoint}/by-ids", productIds, ct);

                if (!response.IsSuccessStatusCode)
                {
                    LogUnexpectedStatusCode(response);
                    return CreateFallbackProducts(productIds);
                }

                var content = await response.Content.ReadFromJsonAsync<List<ProductDto>>(ct);
                if (content is null)
                {
                    LogUnexpectedResponseValue(response);
                    return CreateFallbackProducts(productIds);
                }

                return content;
            },
            onFailure: () => CreateFallbackProducts(productIds));
    }

    #region Helpers

    private async Task<T> ExecuteSafeAsync<T>(Func<Task<T>> operation, Func<T> onFailure)
    {
        try
        {
            return await operation();
        }
        catch (BrokenCircuitException ex) { return HandleError("circuit breaker", ex, onFailure); }
        catch (TimeoutRejectedException ex) { return HandleError("timeout", ex, onFailure); }
        catch (BulkheadRejectedException ex) { return HandleError("bulkhead rejection", ex, onFailure); }
        catch (HttpRequestException ex) { return HandleError("request error", ex, onFailure); }

        throw new InvalidOperationException($"Unexpected error while sending request to {nameof(ProductsServiceClient)}.");
    }

    private T HandleError<T>(string reason, Exception ex, Func<T> fallback)
    {
        _logger.LogError(ex, 
            "{Service} unavailable due to {Reason}.",
            nameof(ProductsServiceClient),
            reason);
        return fallback();
    }

    private void EnsureSuccessResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            LogUnexpectedStatusCode(response);
            throw new HttpRequestException($"Products Service responded with {response.StatusCode}.", null, HttpStatusCode.InternalServerError);
        }
    }

    private void LogUnexpectedStatusCode(HttpResponseMessage response)
    {
        _logger.LogWarning("{Service} responded with {StatusCode} for request {RequestUrl}.",
            nameof(ProductsServiceClient),
            response.StatusCode,
            response.RequestMessage?.RequestUri);
    }

    private void LogUnexpectedResponseValue(HttpResponseMessage response)
    {
        _logger.LogWarning("{Service} returned an empty or invalid response for request {RequestUrl}. Using fallback value.",
            nameof(ProductsServiceClient),
            response.RequestMessage?.RequestUri);
    }

    private static HttpRequestException CreateUnavailableException(string? message = null, Exception? inner = null) =>
        new(message ?? "Products Service is unavailable.", inner, HttpStatusCode.ServiceUnavailable);

    private static List<ProductDto> CreateFallbackProducts(IEnumerable<Guid> ids)
    {
        return ids
            .Select(id => FallbackProductTemplate with { Id = id })
            .ToList();
    }

    #endregion
}
