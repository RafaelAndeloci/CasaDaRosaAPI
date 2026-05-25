using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Admin.Products.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Queries.GetProducts;

public sealed record GetAdminProductsQuery(
    string? Name = null,
    Guid? CategoryId = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<AdminProductResponse>>;
