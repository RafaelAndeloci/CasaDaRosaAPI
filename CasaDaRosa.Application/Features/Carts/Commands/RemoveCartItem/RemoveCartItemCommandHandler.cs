using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Carts.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Commands.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler(
    IUserContext userContext,
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveCartItemCommand, CartResponse>
{
    public async Task<CartResponse> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var cart = await cartRepository.GetByUserIdAsync(userContext.UserId.Value, includeItems: true, cancellationToken: cancellationToken);

        if (cart is null)
        {
            throw new NotFoundApplicationException("cart.not_found", "Cart not found.");
        }

        var result = cart.RemoveItem(request.ItemId);

        if (result.IsFailure)
        {
            throw MapFailure(result.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CartResponseMapper.FromCart(cart);
    }

    private static Exception MapFailure(Domain.Abstractions.Error error)
    {
        return error.Code switch
        {
            "Cart.ItemNotInCart" => new NotFoundApplicationException(error.Code, error.Name),
            "Cart.NotActive" => new UnprocessableApplicationException(error.Code, error.Name),
            _ => new UnprocessableApplicationException(error.Code, error.Name)
        };
    }
}
