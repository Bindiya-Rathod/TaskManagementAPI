using System.Net;
using System.Text.Json;
using TaskManagement.Core.Exceptions;
using TaskManagement.Shared.Models;

namespace TaskManagement.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.TraceIdentifier;
            _logger.LogError(exception, "Error - CorrelationId: {CorrelationId}", correlationId);

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case TaskNotFoundException taskNotFound:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Error.Code = "TASK_NOT_FOUND";
                    errorResponse.Error.Message = taskNotFound.Message;
                    errorResponse.Error.Details.Add($"TaskId: {taskNotFound.TaskId}");
                    break;

                case UserNotFoundException userNotFound:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Error.Code = "USER_NOT_FOUND";
                    errorResponse.Error.Message = userNotFound.Message;
                    errorResponse.Error.Details.Add($"UserId: {userNotFound.UserId}");
                    break;

                case ValidationException validation:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Error.Code = "VALIDATION_ERROR";
                    errorResponse.Error.Message = validation.Message;
                    errorResponse.Error.Details = validation.Errors;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Error.Code = "INTERNAL_SERVER_ERROR";
                    errorResponse.Error.Message = "An error occurred";
                    errorResponse.Error.Details.Add($"CorrelationId: {correlationId}");
                    break;
            }

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(json);
        }
    }
}
