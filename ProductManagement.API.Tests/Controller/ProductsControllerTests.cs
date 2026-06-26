using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Exceptions;

namespace ProductManagement.API.Tests.Controller;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _controller = new ProductsController(_productServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenProductExists()
    {
        _productServiceMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new ProductResponse
            {
                Id = 1,
                ProductName = "Laptop"
            });

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ProductResponse>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal("Laptop", response.Data!.ProductName);
    }

    [Fact]
    public async Task GetById_ShouldThrowNotFound_WhenProductDoesNotExist()
    {
        _productServiceMock
            .Setup(x => x.GetByIdAsync(99))
            .ReturnsAsync((ProductResponse?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _controller.GetById(99));
    }
}
