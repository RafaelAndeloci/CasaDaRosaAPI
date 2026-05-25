using CasaDaRosa.Application.Features.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderResponse>;
