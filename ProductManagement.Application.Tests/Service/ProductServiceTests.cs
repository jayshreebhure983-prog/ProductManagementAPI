using AutoMapper;
using Moq;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _productService = new ProductService(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, ProductName = "Laptop" },
            new() { Id = 2, ProductName = "Mobile" }
        };

        var responses = new List<ProductResponse>
        {
            new() { Id = 1, ProductName = "Laptop" },
            new() { Id = 2, ProductName = "Mobile" }
        };

        var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };

        _repositoryMock.Setup(x => x.GetAllAsync(1, 10)).ReturnsAsync(products);
        _repositoryMock.Setup(x => x.GetTotalCountAsync()).ReturnsAsync(2);
        _mapperMock
            .Setup(x => x.Map<IEnumerable<ProductResponse>>(products))
            .Returns(responses);

        var result = await _productService.GetAllAsync(request);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        var product = new Product { Id = 1, ProductName = "Laptop" };
        var response = new ProductResponse { Id = 1, ProductName = "Laptop" };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        _mapperMock.Setup(x => x.Map<ProductResponse>(product)).Returns(response);

        var result = await _productService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result!.ProductName);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct()
    {
        var request = new CreateProductRequest { ProductName = "Laptop" };
        var product = new Product { ProductName = "Laptop" };

        _mapperMock.Setup(x => x.Map<Product>(request)).Returns(product);

        var result = await _productService.CreateAsync(request);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        var product = new Product { Id = 1, ProductName = "Old Name" };
        var request = new UpdateProductRequest { ProductName = "New Name" };

        _repositoryMock.Setup(x => x.GetTrackedByIdAsync(1)).ReturnsAsync(product);

        await _productService.UpdateAsync(1, request);

        Assert.Equal("New Name", product.ProductName);
        _repositoryMock.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct()
    {
        var product = new Product { Id = 1, ProductName = "Laptop" };

        _repositoryMock.Setup(x => x.GetTrackedByIdAsync(1)).ReturnsAsync(product);

        await _productService.DeleteAsync(1);

        _repositoryMock.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
    }
}
