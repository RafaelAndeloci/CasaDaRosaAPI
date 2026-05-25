using CasaDaRosa.Application.Features.Carts.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(Guid ItemId) : IRequest<CartResponse>;
