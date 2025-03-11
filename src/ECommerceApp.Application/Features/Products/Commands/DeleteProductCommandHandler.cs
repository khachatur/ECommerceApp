using ECommerceApp.Domain.Interfaces;
using MediatR;

namespace ECommerceApp.Application.Features.Products.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IAppDbContext _context;

    public DeleteProductCommandHandler(IProductRepository productRepository, IAppDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _productRepository.DeleteAsync(request.Id);
        await _context.SaveChangesAsync(cancellationToken);
    }
}