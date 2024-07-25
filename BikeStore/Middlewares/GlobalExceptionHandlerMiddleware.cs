using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace BikeStore.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"TraceId: {Activity.Current?.TraceId}\n" + ex.Message);

            var details = new
            {
                Title = "Unexpectec Error",
                StatusCode = HttpStatusCode.InternalServerError,
                TraceId = Activity.Current?.TraceId.ToString() ?? ""
            };

            string body = JsonSerializer.Serialize(details);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsync(body);
        }
    }
}
