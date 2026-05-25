using MediatR;
using CasaDaRosa.Application.Common.Pagination;

namespace CasaDaRosa.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    string? Name = null,
    Guid? CategoryId = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<ProductListItemResponse>>;
