using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManagementAPI.Middleware;
using System.Text.Json;

namespace ProductManagement.API.Tests.Controller;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturnNotFound_ForNotFoundException()
    {
        var middleware = new ExceptionMiddleware(
            _ => throw new ProductManagement.Domain.Exceptions.NotFoundException("Product not found"),
            Mock.Of<ILogger<ExceptionMiddleware>>());

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);

        Assert.Equal("Product not found", body.GetProperty("Message").GetString());
    }
}
