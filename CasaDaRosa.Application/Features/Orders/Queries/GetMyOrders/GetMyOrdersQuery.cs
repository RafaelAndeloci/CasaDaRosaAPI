using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(
    int? StatusId = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<OrderResponse>>;
