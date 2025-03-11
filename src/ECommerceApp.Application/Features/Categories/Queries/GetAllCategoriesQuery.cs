using ECommerceApp.Common.DTOs;
using MediatR;

namespace ECommerceApp.Application.Features.Categories.Queries;

public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
{
}