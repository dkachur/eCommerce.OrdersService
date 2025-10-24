using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace eCommerce.OrdersService.Infrastructure.Policies;

public class HttpPolicyBuilder
{
    private readonly ILogger _logger;
    private readonly string _clientName;

    private readonly List<IAsyncPolicy<HttpResponseMessage>> _policies = [];

    public HttpPolicyBuilder(ILogger logger, string clientName)
    {
        _logger = logger;
        _clientName = clientName;
    }

    public HttpPolicyBuilder WithRetry(int retryCount = 5, double initialDelaySeconds = 1.5d)
    {
        _policies.Add(CreateRetryPolicy(retryCount, initialDelaySeconds));
        return this;
    }

    public HttpPolicyBuilder WithCircuitBreaker(int threshold = 3, int breakDurationSeconds = 40)
    {
        _policies.Add(CreateCircuitBreakerPolicy(threshold, breakDurationSeconds));
        return this;
    }

    public HttpPolicyBuilder WithTimeout(int timeoutMs = 3000)
    {
        _policies.Add(CreateTimeoutPolicy(timeoutMs));
        return this;
    }

    public HttpPolicyBuilder WithBulkhead(int maxParallelization = 2, int maxQueueActions = 40)
    {
        _policies.Add(CreateBulkheadPolicy(maxParallelization, maxQueueActions));
        return this;
    }

    public IAsyncPolicy<HttpResponseMessage> Build() =>
        _policies.Count == 0
            ? Policy.NoOpAsync<HttpResponseMessage>()
            : Policy.WrapAsync(_policies.ToArray());


    private AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(int retryCount, double initialDelaySeconds)
        => Policy
            .HandleResult<HttpResponseMessage>(r =>
                !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromSeconds(Math.Pow(initialDelaySeconds, attempt)),
                onRetry: (outcome, timeSpan, retryAttempt, context) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryAttempt} after {RetryDelay}s for {Client} due to {StatusCode}",
                        retryAttempt,
                        timeSpan.TotalSeconds,
                        _clientName,
                        outcome.Result.StatusCode);
                });


    private AsyncCircuitBreakerPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(int threshold, int breakDurationSeconds)
        => Policy
            .HandleResult<HttpResponseMessage>(r =>
                !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.NotFound)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: threshold,
                durationOfBreak: TimeSpan.FromSeconds(breakDurationSeconds),
                onBreak: (outcome, durationOfBreak) =>
                {
                    _logger.LogWarning(
                        "Circuit breaker for {Client} opened for {Duration}s after {Threshold} failed attempts. Last status: {StatusCode}",
                        _clientName,
                        durationOfBreak.TotalSeconds,
                        threshold,
                        outcome.Result.StatusCode);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker for {Client} reset — requests are allowed again.",
                        _clientName);
                },
                onHalfOpen: () =>
                {
                    _logger.LogInformation("Circuit breaker for {Client} is half-open — testing next request...",
                        _clientName);
                });

    private AsyncTimeoutPolicy<HttpResponseMessage> CreateTimeoutPolicy(int timeoutMs)
        => Policy
            .TimeoutAsync<HttpResponseMessage>(
                timeout: TimeSpan.FromMilliseconds(timeoutMs),
                onTimeoutAsync: (context, timeout, action) =>
                {
                    _logger.LogWarning(
                        "Request timeout for {Client} after {TimeoutMilliseconds} ms.",
                        _clientName,
                        timeout.TotalMilliseconds);

                    return Task.CompletedTask;
                });

    private AsyncBulkheadPolicy<HttpResponseMessage> CreateBulkheadPolicy(int maxParallelization, int maxQueueActions)
        => Policy
            .BulkheadAsync<HttpResponseMessage>(
                maxParallelization: maxParallelization,
                maxQueuingActions: maxQueueActions,
                onBulkheadRejectedAsync: (context) =>
                {
                    _logger.LogWarning("Bulkhead isolation for {Client} is triggered. Cannot send any more requests since the queue is full.",
                        _clientName);

                    return Task.CompletedTask;
                });
}
