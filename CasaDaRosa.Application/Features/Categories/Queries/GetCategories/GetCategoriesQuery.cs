using CasaDaRosa.Application.Common.Pagination;
using MediatR;

namespace CasaDaRosa.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery(
    string? Name = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<CategoryListItemResponse>>;
