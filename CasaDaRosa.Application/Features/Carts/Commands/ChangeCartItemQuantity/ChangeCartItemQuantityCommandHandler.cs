using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Carts.Common;
using CasaDaRosa.Domain.Entities.Carts.Services;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Commands.ChangeCartItemQuantity;

public sealed class ChangeCartItemQuantityCommandHandler(
    IUserContext userContext,
    ICartRepository cartRepository,
    ICartProductEligibilityService productEligibilityService,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeCartItemQuantityCommand, CartResponse>
{
    public async Task<CartResponse> Handle(ChangeCartItemQuantityCommand request, CancellationToken cancellationToken)
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

        var result = cart.ChangeProductQuantity(request.ProductId, request.Quantity, productEligibilityService);

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
            "Cart.ProductNotInCart" => new NotFoundApplicationException(error.Code, error.Name),
            "Cart.NotActive" => new UnprocessableApplicationException(error.Code, error.Name),
            "Cart.InvalidQuantity" => new UnprocessableApplicationException(error.Code, error.Name),
            "cart.product.not_found" => new NotFoundApplicationException(error.Code, error.Name),
            "cart.product.inactive" => new UnprocessableApplicationException(error.Code, error.Name),
            "cart.product.insufficient_stock" => new UnprocessableApplicationException(error.Code, error.Name),
            _ => new UnprocessableApplicationException(error.Code, error.Name)
        };
    }
}
