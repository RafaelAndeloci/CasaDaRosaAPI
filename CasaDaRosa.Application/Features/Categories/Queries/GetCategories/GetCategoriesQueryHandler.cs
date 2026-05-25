using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Filters;
using CasaDaRosa.Application.Common.Pagination;
using MediatR;

namespace CasaDaRosa.Application.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryListItemResponse>>
{
    public async Task<PagedResult<CategoryListItemResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);

        var filteredCategories = categories
            .Where(category => TextFilterUtility.ContainsNormalized(category.Name.ToString(), request.Name))
            .ToArray();

        var totalCount = filteredCategories.Length;

        var pagedCategories = filteredCategories
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(category => new CategoryListItemResponse(
                category.Id,
                category.Name.ToString(),
                category.Description?.ToString(),
                category.IsActive))
            .ToArray();

        return PagedResult<CategoryListItemResponse>.Create(pagedCategories, request.PageNumber, request.PageSize, totalCount);
    }
}
