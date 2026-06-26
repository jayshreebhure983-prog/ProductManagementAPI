using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Data.Repositories;

namespace ProductManagement.Infrastructure.Tests.Repositories;

public class ProductRepositoryTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var context = GetDbContext();

        var repository = new ProductRepository(context);

        var product = new Product
        {
            ProductName = "Laptop"
        };

        // Act
        await repository.AddAsync(product);

        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, context.Products.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        // Arrange
        var context = GetDbContext();

        context.Products.Add(
            new Product
            {
                Id = 1,
                ProductName = "Laptop"
            });

        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Laptop", result.ProductName);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        // Arrange
        var context = GetDbContext();

        context.Products.AddRange(
            new Product { ProductName = "Laptop" },
            new Product { ProductName = "Mobile" });

        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetAllAsync(1, 10);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Update_ShouldModifyProduct()
    {
        // Arrange
        var context = GetDbContext();

        var product = new Product
        {
            Id = 1,
            ProductName = "Laptop"
        };

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        product.ProductName = "Mobile";

        repository.Update(product);

        await context.SaveChangesAsync();

        // Assert
        Assert.Equal("Mobile",
            context.Products.First().ProductName);
    }

    [Fact]
    public async Task Delete_ShouldRemoveProduct()
    {
        // Arrange
        var context = GetDbContext();

        var product = new Product
        {
            Id = 1,
            ProductName = "Laptop"
        };

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        repository.Delete(product);

        await context.SaveChangesAsync();

        // Assert
        Assert.Empty(context.Products);
    }
}