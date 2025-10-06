using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Users;

public class UsersServiceClient : IUsersServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersServiceClient> _logger;

    public UsersServiceClient(HttpClient httpClient, ILogger<UsersServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync($"/api/users/{userId}", ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error while sending request to Users microservice.");

            throw new HttpRequestException(
                "Users Service is unavailable.",
                inner: null,
                statusCode: HttpStatusCode.ServiceUnavailable);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                message: "Users Service responded with an error.",
                inner: null,
                statusCode: HttpStatusCode.InternalServerError);

        return await response.Content.ReadFromJsonAsync<UserDto>(ct);
    }
}
