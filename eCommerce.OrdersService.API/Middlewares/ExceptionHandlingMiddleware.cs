using Microsoft.AspNetCore.Mvc;

namespace eCommerce.OrdersService.API.Middlewares
{
    /// <summary>
    /// The exception handling middleware that handles all unhandled expections.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Logs all unhandled exceptions and creates response with exception message as detail of the <see cref="ProblemDetails"/> and status code HTTP 500.
        /// </summary>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                if (ex.InnerException is not null)
                    _logger.LogError(ex.InnerException, "Unhandled exception occurred");

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
                {
                    Title = "An unexcepcted error occurred.",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = httpContext.Request.Path
                });
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
