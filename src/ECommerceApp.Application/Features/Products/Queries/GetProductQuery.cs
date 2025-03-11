using ECommerceApp.Common.DTOs;
using MediatR;

namespace ECommerceApp.Application.Features.Products.Queries;

public class GetProductQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}