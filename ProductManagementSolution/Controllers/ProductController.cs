using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Exceptions;

namespace ProductManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Returns a paginated list of products.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProductResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
    {
        var products = await _productService.GetAllAsync(request);

        return Ok(new ApiResponse<PaginatedResponse<ProductResponse>>
        {
            Success = true,
            Message = "Products fetched successfully",
            Data = products
        });
    }

    /// <summary>
    /// Returns a single product by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            throw new NotFoundException($"Product with id {id} was not found.");

        return Ok(new ApiResponse<ProductResponse>
        {
            Success = true,
            Message = "Product fetched successfully",
            Data = product
        });
    }

    /// <summary>
    /// Returns items related to a product.
    /// </summary>


    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var id = await _productService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id, version = "1.0" },
            new ApiResponse<object>
            {
                Success = true,
                Message = "Product created successfully",
                Data = new { Id = id }
            });
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateProductRequest request)
    {
        await _productService.UpdateAsync(id, request);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Product updated successfully"
        });
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Product Delted successfully"
        });
    }
}
