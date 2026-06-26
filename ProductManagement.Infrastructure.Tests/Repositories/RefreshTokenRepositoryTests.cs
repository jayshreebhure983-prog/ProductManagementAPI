using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Data.Repositories;
using Xunit;

namespace ProductManagement.Infrastructure.Tests.Repositories;

public class RefreshTokenRepositoryTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddRefreshToken()
    {
        // Arrange
        var context = GetDbContext();

        var repository = new RefreshTokenRepository(context);

        var refreshToken = new RefreshToken
        {
            Token = "token123",
            UserName = "Admin",
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        await repository.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await context.RefreshTokens.CountAsync());
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldReturnRefreshToken()
    {
        // Arrange
        var context = GetDbContext();

        var refreshToken = new RefreshToken
        {
            Token = "token123",
            UserName = "Admin",
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetByTokenAsync("token123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("token123", result.Token);
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Arrange
        var context = GetDbContext();

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetByTokenAsync("invalid-token");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ShouldRemoveRefreshToken()
    {
        // Arrange
        var context = GetDbContext();

        var refreshToken = new RefreshToken
        {
            Token = "token123",
            UserName = "Admin",
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();

        var repository = new RefreshTokenRepository(context);

        // Act
        repository.Delete(refreshToken);
        await context.SaveChangesAsync();

        // Assert
        Assert.Empty(context.RefreshTokens);
    }
}