using AutoMapper;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Exceptions;

namespace ProductManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<ProductResponse>> GetAllAsync(
        PaginationRequest request)
    {
        var products = await _repository.GetAllAsync(
            request.PageNumber,
            request.PageSize);

        var totalCount = await _repository.GetTotalCountAsync();

        return new PaginatedResponse<ProductResponse>
        {
            Items = _mapper.Map<IEnumerable<ProductResponse>>(products),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return null;

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<int> CreateAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);

        product.CreatedBy = "Admin";
        product.CreatedOn = DateTime.UtcNow;

        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return product.Id;
    }

    public async Task UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = await _repository.GetTrackedByIdAsync(id);

        if (product == null)
            throw new NotFoundException($"Product with id {id} was not found.");

        product.ProductName = request.ProductName;
        product.ModifiedBy = "Admin";
        product.ModifiedOn = DateTime.UtcNow;

        _repository.Update(product);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repository.GetTrackedByIdAsync(id);

        if (product == null)
            throw new NotFoundException($"Product with id {id} was not found.");

        _repository.Delete(product);

        await _unitOfWork.SaveChangesAsync();
    }
}
