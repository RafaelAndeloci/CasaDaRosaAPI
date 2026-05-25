using CasaDaRosa.Application.Features.Carts.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Commands.AddItemToCart;

public sealed record AddItemToCartCommand(Guid ProductId, int Quantity) : IRequest<CartResponse>;
