using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrders;

public sealed class GetAdminOrdersQueryHandler(
    IUserContext userContext,
    IOrderRepository orderRepository) : IRequestHandler<GetAdminOrdersQuery, PagedResult<AdminOrderListItemResponse>>
{
    public async Task<PagedResult<AdminOrderListItemResponse>> Handle(GetAdminOrdersQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var orders = await orderRepository.GetAllAsync(cancellationToken);

        var filteredOrders = orders
            .Where(order => request.UserId is null || order.UserId == request.UserId.Value)
            .Where(order => request.StatusId is null || (int)order.Status == request.StatusId.Value)
            .Where(order => request.PaymentMethodId is null || (int)order.PaymentMethod == request.PaymentMethodId.Value)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToArray();

        var totalCount = filteredOrders.Length;

        var pagedOrders = filteredOrders
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(AdminOrderResponseMapper.ToListItem)
            .ToArray();

        return PagedResult<AdminOrderListItemResponse>.Create(pagedOrders, request.PageNumber, request.PageSize, totalCount);
    }
}
