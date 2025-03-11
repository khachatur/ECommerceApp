using MediatR;

namespace ECommerceApp.Application.Features.Products.Commands;

public class UpdateProductCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
}