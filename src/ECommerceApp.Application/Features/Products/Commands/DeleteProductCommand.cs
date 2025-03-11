using MediatR;

namespace ECommerceApp.Application.Features.Products.Commands;

public class DeleteProductCommand : IRequest
{
    public int Id { get; set; }
}
