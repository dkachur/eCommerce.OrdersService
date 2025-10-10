using eCommerce.OrdersService.Infrastructure.ExternalServices.Users;
using Microsoft.Extensions.Logging;
using Polly;

namespace eCommerce.OrdersService.Infrastructure.Policies;

public class UsersMicroservicePolicy
{
    public IAsyncPolicy<HttpResponseMessage> Wrap { get; }

    private const int RetryCount = 5;
    private const int CircuitBreakerThreshold = 3;
    private const int DurationOfBreakInSeconds = 40;
    private const double InitialDelaySeconds = 1.5d;

    public UsersMicroservicePolicy(ILogger<UsersServiceClient> logger)
    {
        var retry = GetRetryPolicy(logger);
        var breaker = GetCircuitBreakerPolicy(logger);

        Wrap = Policy.WrapAsync(retry, breaker);
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger<UsersServiceClient> logger)
    {
        var policy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                retryCount: RetryCount,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromSeconds(Math.Pow(InitialDelaySeconds, attempt)),
                onRetry: (outcome, timeSpan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        "Retry {RetryAttempt} after {RetryDelay} seconds for UsersServiceClient due to {StatusCode}",
                        retryAttempt,
                        timeSpan.TotalSeconds,
                        outcome.Result.StatusCode);
                });

        return policy;
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger<UsersServiceClient> logger)
    {
        var policy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.NotFound)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(DurationOfBreakInSeconds),
                onBreak: (outcome, durationOfBreak) =>
                {
                    logger.LogWarning(
                        "Circuit breaker for UsersServiceClient opened for {Duration}s after {Attempts} failed attempts. Last status: {StatusCode}",
                        durationOfBreak.TotalSeconds,
                        CircuitBreakerThreshold,
                        outcome.Result.StatusCode);
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker for UsersServiceClient reset — requests are allowed again.");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker for UsersServiceClient is half-open — testing next request...");
                });

        return policy;
    }
}
