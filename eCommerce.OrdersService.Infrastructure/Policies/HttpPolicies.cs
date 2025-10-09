using eCommerce.OrdersService.Infrastructure.ExternalServices.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace eCommerce.OrdersService.Infrastructure.Policies;

public static class HttpPolicies
{
    private const int RetryCount = 5;
    private const int CircuitBreakerThreshold = 3;
    private const int DurationOfBreakInSeconds = 15;
    private const double InitialDelaySeconds = 1.5d;

    public static IAsyncPolicy<HttpResponseMessage> GetUsersServiceRetryPolicy(IServiceProvider sp)
    {
        var logger = sp.GetRequiredService<ILogger<UsersServiceClient>>();
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
                        "Retry {RetryAttempt} after {RetryDelay} seconds for UsersService due to {StatusCode}",
                        retryAttempt,
                        timeSpan.TotalSeconds,
                        outcome.Result.StatusCode);
                });

        return policy;
    }

    public static IAsyncPolicy<HttpResponseMessage> GetUsersServiceCircuitBreakerPolicy(IServiceProvider sp)
    {
        var logger = sp.GetRequiredService<ILogger<UsersServiceClient>>();
        var policy = Policy
            .HandleResult<HttpResponseMessage>(r => 
                !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.NotFound)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(DurationOfBreakInSeconds),
                onBreak: (outcome, durationOfBreak) =>
                {
                    logger.LogWarning(
                        "Circuit breaker for UsersService opened for {Duration}s after {Attempts} failed attempts. Last status: {StatusCode}",
                        durationOfBreak,
                        CircuitBreakerThreshold,
                        outcome.Result.StatusCode);
                },
                onReset: () =>
                {
                    logger.LogInformation(
                        "Circuit breaker for UsersService reset — requests are allowed again.");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker for UsersService is half-open — testing next request...");
                });

        return policy;
    }
}
