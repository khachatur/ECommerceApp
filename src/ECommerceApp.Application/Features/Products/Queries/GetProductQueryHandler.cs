using ECommerceApp.Common.DTOs;
using ECommerceApp.Domain.Interfaces;
using MediatR;

namespace ECommerceApp.Application.Features.Products.Queries;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null) throw new Exception("Product not found");
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            CategoryId = product.CategoryId
        };
    }
}