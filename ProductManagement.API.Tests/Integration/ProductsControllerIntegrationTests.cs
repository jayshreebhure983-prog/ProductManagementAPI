using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductManagement.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProductManagement.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        return host;
    }
}

public class ProductsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { UserName = "admin", Password = "admin123" });

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, body);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.True(content.TryGetProperty("accessToken", out _));
        Assert.True(content.TryGetProperty("refreshToken", out _));
    }

    [Fact]
    public async Task GetProducts_ShouldReturnUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/api/v1/products");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenAuthenticated()
    {
        var token = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/products");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { ProductName = "Integration Test Product" });

        var response = await _client.SendAsync(request);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { UserName = "admin", Password = "admin123" });

        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        return loginContent.GetProperty("accessToken").GetString()!;
    }
}
