using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using MediatR;

namespace ECommerceApp.Application.Features.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _productRepository;
    private readonly IAppDbContext _context;

    public CreateProductCommandHandler(IProductRepository productRepository, IAppDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
            CategoryId = request.CategoryId
        };
        await _productRepository.AddAsync(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
