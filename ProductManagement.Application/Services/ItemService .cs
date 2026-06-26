using AutoMapper;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Exceptions;

public class ItemService : IItemService
{
    private readonly IItemRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ItemService(
        IItemRepository repository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemResponse>> GetByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
            throw new NotFoundException($"Product {productId} not found.");

        var items = await _repository.GetByProductIdAsync(productId);

        return _mapper.Map<IEnumerable<ItemResponse>>(items);
    }

    public async Task<ItemResponse?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);

        return item == null
            ? null
            : _mapper.Map<ItemResponse>(item);
    }

    public async Task<int> CreateAsync(int productId, CreateItemRequest request)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
            throw new NotFoundException($"Product {productId} not found.");

        var item = new Item
        {
            ProductId = productId,
            Quantity = request.Quantity
        };

        await _repository.AddAsync(item);

        await _unitOfWork.SaveChangesAsync();

        return item.Id;
    }

    public async Task UpdateAsync(int id, UpdateItemRequest request)
    {
        var item = await _repository.GetTrackedByIdAsync(id);

        if (item == null)
            throw new NotFoundException($"Item {id} not found.");

        item.Quantity = request.Quantity;

        _repository.Update(item);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _repository.GetTrackedByIdAsync(id);

        if (item == null)
            throw new NotFoundException($"Item {id} not found.");

        _repository.Delete(item);

        await _unitOfWork.SaveChangesAsync();
    }
}