using ProductManagement.Application.DTOs;

public interface IItemService
{
    Task<IEnumerable<ItemResponse>> GetByProductIdAsync(int productId);

    Task<ItemResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(int productId, CreateItemRequest request);

    Task UpdateAsync(int id, UpdateItemRequest request);

    Task DeleteAsync(int id);
}