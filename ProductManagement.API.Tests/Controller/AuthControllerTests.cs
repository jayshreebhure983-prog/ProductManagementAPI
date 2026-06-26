using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        var refreshTokenServiceMock = new Mock<IRefreshTokenService>();

        refreshTokenServiceMock
            .Setup(x => x.LoginAsync("admin"))
            .ReturnsAsync(new LoginResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            });

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
            { "AdminCredentials:UserName", "admin" },
            { "AdminCredentials:Password", "admin123" }
            })
            .Build();

        var controller = new AuthController(
            refreshTokenServiceMock.Object,
            configuration);

        var result = await controller.Login(new LoginRequest
        {
            UserName = "admin",
            Password = "admin123"
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);

        Assert.Equal("access-token", response.AccessToken);
    }
}