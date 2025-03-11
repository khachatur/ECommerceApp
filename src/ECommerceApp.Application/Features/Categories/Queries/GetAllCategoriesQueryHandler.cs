using ECommerceApp.Common.DTOs;
using ECommerceApp.Domain.Interfaces;
using MediatR;

namespace ECommerceApp.Application.Features.Categories.Queries;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(ICategoryRepository CategoryRepository)
    {
        _categoryRepository = CategoryRepository;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(p => new CategoryDto
        {
            Id = p.Id,
            Name = p.Name
        }).ToList();
    }
}