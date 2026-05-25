using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(IUserContext userContext, IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null || order.UserId != userContext.UserId.Value)
        {
            throw new NotFoundApplicationException("orders.not_found", "Order not found for the authenticated user.");
        }

        return OrderResponseMapper.FromOrder(order);
    }
}
