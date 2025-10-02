using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using System.Net;
using System.Net.Http.Json;

namespace eCommerce.OrdersService.Infrastructure.Clients;

public class UsersServiceClient : IUsersServiceClient
{
    private readonly HttpClient _httpClient;

    public UsersServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync($"/api/users/{userId}", ct);
        }
        catch (HttpRequestException e)
        {
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
