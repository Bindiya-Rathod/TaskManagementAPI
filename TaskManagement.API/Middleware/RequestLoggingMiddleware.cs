namespace TaskManagement.API.Middleware
{
    /// <summary>
    /// Middleware for logging requests with correlation IDs
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.TraceIdentifier;

            _logger.LogInformation(
                "Request: {Method} {Path} - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            await _next(context);

            _logger.LogInformation(
                "Response: {StatusCode} - CorrelationId: {CorrelationId}",
                context.Response.StatusCode,
                correlationId);
        }
    }

}
