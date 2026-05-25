using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Orders.Common;
using CasaDaRosa.Domain.Entities.Orders;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Orders.Commands.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler(
    IUserContext userContext,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderStatusCommand, AdminOrderResponse>
{
    public async Task<AdminOrderResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var order = await orderRepository.GetTrackedByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new NotFoundApplicationException("orders.not_found", "Order not found.");
        }

        var targetStatus = (OrderStatus)request.StatusId;
        var result = order.UpdateStatus(targetStatus);

        if (result.IsFailure)
        {
            throw new UnprocessableApplicationException(result.Error.Code, result.Error.Name);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdminOrderResponseMapper.ToResponse(order);
    }
}
