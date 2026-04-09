using System.Net;
using System.Text.Json;
using TaxAccount.Exceptions;

namespace TaxAccount.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, 
            ILogger<ExceptionMiddleware> logger)
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
            catch (AppException ex)
            {
                // Change ERR to WRN for known exceptions
                if (ex.StatusCode >= 500)
                  _logger.LogWarning(ex, "Application exception occurred");
                else
                  _logger.LogWarning("Application exception occurred: {Message}", ex.Message);

                await HandleExceptionAsync(context, ex.Message, ex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception occurred");
                await HandleExceptionAsync(context, 
                    "An unexpected error occurred", 
                    (int)HttpStatusCode.InternalServerError);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = statusCode,
                message = message,
                traceId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}