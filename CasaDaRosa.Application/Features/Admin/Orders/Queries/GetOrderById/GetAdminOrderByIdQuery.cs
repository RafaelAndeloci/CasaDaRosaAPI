using CasaDaRosa.Application.Features.Admin.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrderById;

public sealed record GetAdminOrderByIdQuery(Guid OrderId) : IRequest<AdminOrderResponse>;
