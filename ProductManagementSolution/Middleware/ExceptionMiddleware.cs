using ProductManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ProductManagementAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string message;

        switch (exception)
        {
            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case ValidationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case UnauthorizedException:
            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = context.RequestServices
                    .GetService<IHostEnvironment>()?
                    .IsDevelopment() == true
                    || context.RequestServices
                        .GetService<IHostEnvironment>()?
                        .IsEnvironment("Testing") == true
                    ? exception.Message
                    : "An unexpected error occurred.";
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = new
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
