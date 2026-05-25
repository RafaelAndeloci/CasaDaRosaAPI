using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Admin.Categories.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategories;

public sealed record GetAdminCategoriesQuery(
    string? Name = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<AdminCategoryResponse>>;
