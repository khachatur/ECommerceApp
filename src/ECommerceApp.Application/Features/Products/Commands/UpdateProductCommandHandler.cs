using ECommerceApp.Domain.Interfaces;
using MediatR;

namespace ECommerceApp.Application.Features.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IAppDbContext _context;

    public UpdateProductCommandHandler(IProductRepository productRepository, IAppDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null) throw new Exception("Product not found");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.CategoryId = request.CategoryId;

        await _productRepository.UpdateAsync(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
