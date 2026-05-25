using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler(IUserContext userContext, IOrderRepository orderRepository) : IRequestHandler<GetMyOrdersQuery, PagedResult<OrderResponse>>
{
    public async Task<PagedResult<OrderResponse>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var orders = await orderRepository.GetByUserIdAsync(userContext.UserId.Value, cancellationToken);

        var filteredOrders = orders
            .Where(order => !request.StatusId.HasValue || (int)order.Status == request.StatusId.Value)
            .ToArray();

        var totalCount = filteredOrders.Length;

        var pagedOrders = filteredOrders
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(OrderResponseMapper.FromOrder)
            .ToArray();

        return PagedResult<OrderResponse>.Create(pagedOrders, request.PageNumber, request.PageSize, totalCount);
    }
}
