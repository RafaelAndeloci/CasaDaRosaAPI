using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Carts.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Queries.GetMyCart;

public sealed class GetMyCartQueryHandler(IUserContext userContext, ICartRepository cartRepository) : IRequestHandler<GetMyCartQuery, CartResponse>
{
    public async Task<CartResponse> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var cart = await cartRepository.GetByUserIdAsync(userContext.UserId.Value, includeItems: true, cancellationToken: cancellationToken);

        return cart is null
            ? CartResponseMapper.Empty()
            : CartResponseMapper.FromCart(cart);
    }
}
