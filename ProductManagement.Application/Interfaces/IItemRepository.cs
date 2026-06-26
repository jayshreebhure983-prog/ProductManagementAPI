using ProductManagement.Domain.Entities;

public interface IItemRepository
{
    Task<IEnumerable<Item>> GetByProductIdAsync(int productId);

    Task<Item?> GetByIdAsync(int id);

    Task<Item?> GetTrackedByIdAsync(int id);

    Task AddAsync(Item item);

    void Update(Item item);

    void Delete(Item item);
}