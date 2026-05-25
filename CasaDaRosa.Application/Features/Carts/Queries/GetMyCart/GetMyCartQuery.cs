using CasaDaRosa.Application.Features.Carts.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Queries.GetMyCart;

public sealed record GetMyCartQuery() : IRequest<CartResponse>;
