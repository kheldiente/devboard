using FluentValidation;
using System.Net;
using System.Text.Json;

namespace DevBoard.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validation failed: {Errors}", ex.Message);
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 400,
                title = "Validation failed",
                errors
            }));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning("Not found: {Message}", ex.Message);
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 404,
                title = ex.Message
            }));
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Unauthorized: {Message}", ex.Message);
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 401,
                title = ex.Message
            }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 500,
                title = "An unexpected error occurred"
            }));
        }
    }
}