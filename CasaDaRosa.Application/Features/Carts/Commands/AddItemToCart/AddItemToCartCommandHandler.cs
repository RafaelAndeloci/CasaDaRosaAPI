using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Carts.Common;
using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.Entities.Carts.Services;
using MediatR;

namespace CasaDaRosa.Application.Features.Carts.Commands.AddItemToCart;

public sealed class AddItemToCartCommandHandler(
    IUserContext userContext,
    ICartRepository cartRepository,
    IProductRepository productRepository,
    ICartProductEligibilityService productEligibilityService,
    IUnitOfWork unitOfWork) : IRequestHandler<AddItemToCartCommand, CartResponse>
{
    public async Task<CartResponse> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundApplicationException("products.not_found", "Product not found.");
        }

        var cart = await cartRepository.GetByUserIdAsync(userContext.UserId.Value, includeItems: true, cancellationToken: cancellationToken)
            ?? Cart.Create(userContext.UserId.Value, CartStatus.Empty, []);

        var cartItem = CartItem.Create(cart.Id, product.Id, request.Quantity, product.Price);
        var result = cart.AddItem(cartItem, productEligibilityService);

        if (result.IsFailure)
        {
            throw MapFailure(result.Error);
        }

        if (cart.Items.Count == 1)
        {
            await cartRepository.AddAsync(cart, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CartResponseMapper.FromCart(cart);
    }

    private static Exception MapFailure(Domain.Abstractions.Error error)
    {
        return error.Code switch
        {
            "Cart.DuplicatedItem" => new ConflictApplicationException(error.Code, error.Name),
            "Cart.NotActive" => new UnprocessableApplicationException(error.Code, error.Name),
            "cart.product.not_found" => new NotFoundApplicationException(error.Code, error.Name),
            "cart.product.inactive" => new UnprocessableApplicationException(error.Code, error.Name),
            "cart.product.insufficient_stock" => new UnprocessableApplicationException(error.Code, error.Name),
            _ => new UnprocessableApplicationException(error.Code, error.Name)
        };
    }
}
