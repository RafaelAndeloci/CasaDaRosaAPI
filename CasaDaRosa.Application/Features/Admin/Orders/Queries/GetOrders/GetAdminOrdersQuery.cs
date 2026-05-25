using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Admin.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrders;

public sealed record GetAdminOrdersQuery(
    Guid? UserId = null,
    int? StatusId = null,
    int? PaymentMethodId = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<AdminOrderListItemResponse>>;
