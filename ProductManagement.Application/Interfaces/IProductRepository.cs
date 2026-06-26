using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize);

    Task<int> GetTotalCountAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<Product?> GetTrackedByIdAsync(int id);

    Task<Product?> GetByNameAsync(string name);

    Task AddAsync(Product product);

    void Update(Product product);

    void Delete(Product product);
}
