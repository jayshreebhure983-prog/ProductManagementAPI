using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Data.Repositories;
using ProductManagement.Infrastructure.Identity;

namespace ProductManagement.Application.Tests.Service;

public class RefreshTokenServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task LoginAsync_ShouldPersistRefreshToken()
    {
        var context = GetDbContext();
        var jwtMock = new Mock<IJwtTokenService>();
        jwtMock.Setup(x => x.GenerateAccessToken("admin")).Returns("access-token");

        var service = new RefreshTokenService(
            new RefreshTokenRepository(context),
            jwtMock.Object,
            new UnitOfWork(context),
            Options.Create(new JwtSettings { RefreshTokenDurationInDays = 7 }));

        var response = await service.LoginAsync("admin");

        Assert.Equal("access-token", response.AccessToken);
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        Assert.Equal(1, await context.RefreshTokens.CountAsync());
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldRotateToken()
    {
        var context = GetDbContext();
        var jwtMock = new Mock<IJwtTokenService>();
        jwtMock.Setup(x => x.GenerateAccessToken("admin")).Returns("new-access-token");

        var repository = new RefreshTokenRepository(context);
        var unitOfWork = new UnitOfWork(context);

        context.RefreshTokens.Add(new RefreshToken
        {
            Token = "old-refresh-token",
            UserName = "admin",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        });

        await context.SaveChangesAsync();

        var service = new RefreshTokenService(
            repository,
            jwtMock.Object,
            unitOfWork,
            Options.Create(new JwtSettings { RefreshTokenDurationInDays = 7 }));

        var response = await service.RefreshTokenAsync("old-refresh-token");

        Assert.Equal("new-access-token", response.AccessToken);
        Assert.NotEqual("old-refresh-token", response.RefreshToken);

        var oldToken = await context.RefreshTokens
            .FirstAsync(x => x.Token == "old-refresh-token");

        Assert.True(oldToken.IsRevoked);
    }
}
