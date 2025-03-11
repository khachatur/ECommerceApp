using ECommerceApp.Common.DTOs;
using MediatR;

namespace ECommerceApp.Application.Features.Products.Queries;

public class GetAllProductsQuery : IRequest<List<ProductDto>>
{
}