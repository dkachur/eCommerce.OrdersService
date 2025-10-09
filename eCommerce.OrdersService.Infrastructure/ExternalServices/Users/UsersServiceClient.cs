using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

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
            return new UserDto(
                UserId: Guid.Empty,
                Email: "Temporarily unavailable",
                PersonName: "Temporarily unavailable",
                Gender: "Temporarily unavailable");

            //throw new HttpRequestException(
            //    message: "Users Service responded with an error.",
            //    inner: null,
            //    statusCode: HttpStatusCode.InternalServerError);

        return await response.Content.ReadFromJsonAsync<UserDto>(ct);
    }

    public async Task<List<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsJsonAsync($"/api/users/by-ids", userIds, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error while sending request to Users microservice.");

            throw new HttpRequestException(
                "Users Service is unavailable.",
                inner: null,
                statusCode: HttpStatusCode.ServiceUnavailable);
        }

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                message: "Users Service responded with an error.",
                inner: null,
                statusCode: HttpStatusCode.InternalServerError);

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>(ct);
        if (users is null)
        {
            _logger.LogError("Users Service returned an empty or invalid response.");
            throw new InvalidDataException("Invalid response from Users Service.");
        }

        return users;
    }
}
