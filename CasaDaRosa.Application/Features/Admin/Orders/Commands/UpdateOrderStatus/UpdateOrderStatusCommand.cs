using CasaDaRosa.Application.Features.Admin.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Orders.Commands.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    int StatusId) : IRequest<AdminOrderResponse>;
