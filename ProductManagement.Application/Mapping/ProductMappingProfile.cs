using AutoMapper;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponse>()
            .ForMember(
                dest => dest.Items,
                opt => opt.MapFrom(src => src.Items));

        CreateMap<Item, ItemResponse>();

        CreateMap<CreateProductRequest, Product>();

        CreateMap<UpdateProductRequest, Product>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
        CreateMap<Item, ItemResponse>();

        CreateMap<CreateItemRequest, Item>();

        CreateMap<UpdateItemRequest, Item>();
    }
}
