using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrderById;

public sealed class GetAdminOrderByIdQueryHandler(
    IUserContext userContext,
    IOrderRepository orderRepository) : IRequestHandler<GetAdminOrderByIdQuery, AdminOrderResponse>
{
    public async Task<AdminOrderResponse> Handle(GetAdminOrderByIdQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new NotFoundApplicationException("orders.not_found", "Order not found.");
        }

        return AdminOrderResponseMapper.ToResponse(order);
    }
}
