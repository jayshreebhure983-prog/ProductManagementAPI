using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResponse<ProductResponse>> GetAllAsync(PaginationRequest request);

    Task<ProductResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateProductRequest request);

    Task UpdateAsync(int id, UpdateProductRequest request);

    Task DeleteAsync(int id);
}
