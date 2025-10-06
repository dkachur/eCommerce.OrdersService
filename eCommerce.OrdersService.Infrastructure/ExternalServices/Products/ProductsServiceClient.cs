using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Products.DTOs;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Products;

public class ProductsServiceClient : IProductsServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsServiceClient> _logger;

    public ProductsServiceClient(HttpClient httpClient, ILogger<ProductsServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Dictionary<Guid, bool>> CheckProductsExistAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsync("/api/products/exists", JsonContent.Create(ids), ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error while sending request to Products microservice.");

            throw new HttpRequestException(
                "Products Service is unavailable.",
                inner: null,
                statusCode: HttpStatusCode.ServiceUnavailable);
        }

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                message: "Products Service responded with an error.",
                inner: null,
                statusCode: HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadFromJsonAsync<IEnumerable<ProductExistenceResponse>>(ct);
        if (content is null)
        {
            _logger.LogError("Products Service returned an empty or invalid response.");
            throw new HttpRequestException(
                "Invalid response from Products Service.",
                inner: null,
                statusCode: HttpStatusCode.InternalServerError);
        }

        var dictionary = content
            .GroupBy(p => p.ProductId)
            .Select(g => g.First())
            .ToDictionary(p => p.ProductId, p => p.Exists);

        return dictionary;
    }
}
