using CasaDaRosa.Application.Features.Carts.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Commands.ChangeCartItemQuantity;

public sealed record ChangeCartItemQuantityCommand(Guid ProductId, int Quantity) : IRequest<CartResponse>;
