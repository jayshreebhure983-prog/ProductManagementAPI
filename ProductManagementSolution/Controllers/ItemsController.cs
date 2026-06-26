using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products/{productId:int}/items")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IItemService _service;

    public ItemsController(IItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int productId)
    {
        var items = await _service.GetByProductIdAsync(productId);

        return Ok(new ApiResponse<IEnumerable<ItemResponse>>
        {
            Success = true,
            Message = "Items fetched successfully",
            Data = items
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);

        if (item == null)
            return NotFound();

        return Ok(new ApiResponse<ItemResponse>
        {
            Success = true,
            Message = "Item fetched successfully",
            Data = item
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        int productId,
        CreateItemRequest request)
    {
        var id = await _service.CreateAsync(productId, request);

        return CreatedAtAction(
            nameof(GetById),
            new { productId, id, version = "1.0" },
            new { Id = id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        UpdateItemRequest request)
    {
        await _service.UpdateAsync(id, request);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Item Updated Successfully"
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Item Deleted Successfully"
        });
    }
}