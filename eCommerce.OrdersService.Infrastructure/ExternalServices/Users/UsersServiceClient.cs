using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.ServiceContracts;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net;
using System.Net.Http.Json;

namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Users;

public class UsersServiceClient : IUsersServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersServiceClient> _logger;

    private const string UsersEndpoint = "/api/users";
    private const string UnavailableText = "Temporarily unavailable";
    private static readonly UserDto FallbackUserTemplate = new(
        Guid.Empty,
        UnavailableText,
        UnavailableText,
        UnavailableText);

    public UsersServiceClient(HttpClient httpClient, ILogger<UsersServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CheckUserExistsAsync(Guid userId, CancellationToken ct = default)
    {
         return await ExecuteSafeAsync(
            operation: async () =>
            {
                var response = await _httpClient.GetAsync($"{UsersEndpoint}/{userId}", ct);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return false;

                EnsureSuccessResponse(response);

                var user = await response.Content.ReadFromJsonAsync<UserDto>(ct);
                return user is not null;
            },
            onFailure: () => throw CreateUnavailableException("Unable to verify user existence."));
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        return await ExecuteSafeAsync<UserDto?>(
            operation: async () =>
            {
                var response = await _httpClient.GetAsync($"{UsersEndpoint}/{userId}", ct);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                if (!response.IsSuccessStatusCode)
                {
                    LogUnexpectedStatusCode(response);
                    return CreateFallbackUserWithId(userId);
                }

                var user = await response.Content.ReadFromJsonAsync<UserDto>(ct);
                if (user is null)
                {
                    LogUnexpectedResponseValue(response);
                    return CreateFallbackUserWithId(userId);
                }

                return user;
            },
            onFailure: () => CreateFallbackUserWithId(userId));
    }

    public async Task<List<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        return await ExecuteSafeAsync(
            operation: async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{UsersEndpoint}/by-ids", userIds, ct);

                if (!response.IsSuccessStatusCode)
                {
                    LogUnexpectedStatusCode(response);
                    return CreateFallbackUsers(userIds);
                }

                var users = await response.Content.ReadFromJsonAsync<List<UserDto>>(ct);
                if (users is null)
                {
                    LogUnexpectedResponseValue(response);
                    return CreateFallbackUsers(userIds);
                }

                return users;
            },
            onFailure: () => CreateFallbackUsers(userIds));
    }

    #region Helpers

    private async Task<T> ExecuteSafeAsync<T>(Func<Task<T>> operation, Func<T> onFailure)
    {
        try
        {
            return await operation();
        }
        catch (BrokenCircuitException ex) { return HandleErrorAsync("circuit breaker", ex, onFailure); }
        catch (TimeoutRejectedException ex) { return HandleErrorAsync("timeout", ex, onFailure); }
        catch (BulkheadRejectedException ex) { return HandleErrorAsync("bulkhead rejection", ex, onFailure); }
        catch (HttpRequestException ex) { return HandleErrorAsync("request error", ex, onFailure); }

        throw new InvalidOperationException($"Unexpected error while sending request to {nameof(UsersServiceClient)}.");
    }

    private T HandleErrorAsync<T>(string reason, Exception ex, Func<T> fallback)
    {
        _logger.LogError(ex, 
            "{Service} unavailable due to {Reason}.",
            nameof(UsersServiceClient),
            reason);
        return fallback();
    }

    private void EnsureSuccessResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            LogUnexpectedStatusCode(response);
            throw new HttpRequestException($"Users Service responded with {response.StatusCode}.", null, HttpStatusCode.InternalServerError);
        }
    }

    private void LogUnexpectedStatusCode(HttpResponseMessage response)
    {
        _logger.LogWarning("{Service} responded with {StatusCode} for request {RequestUrl}.",
            nameof(UsersServiceClient),
            response.StatusCode,
            response.RequestMessage?.RequestUri);
    }

    private void LogUnexpectedResponseValue(HttpResponseMessage response)
    {
        _logger.LogWarning("{Service} returned an empty or invalid response for request {RequestUrl}. Using fallback value.",
            nameof(UsersServiceClient),
            response.RequestMessage?.RequestUri);
    }

    private static HttpRequestException CreateUnavailableException(string? message = null, Exception? inner = null) =>
        new(message ?? "Users Service is unavailable.", inner, HttpStatusCode.ServiceUnavailable);

    private static List<UserDto> CreateFallbackUsers(IEnumerable<Guid> ids)
        => ids.Select(CreateFallbackUserWithId).ToList();

    private static UserDto CreateFallbackUserWithId(Guid userId) => FallbackUserTemplate with { UserId = userId };

    #endregion
}
