using ECommerceApp.Common.DTOs;
using ECommerceApp.Domain.Interfaces;
using MediatR;

namespace ECommerceApp.Application.Features.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Quantity = p.Quantity,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name
        }).ToList();
    }
}