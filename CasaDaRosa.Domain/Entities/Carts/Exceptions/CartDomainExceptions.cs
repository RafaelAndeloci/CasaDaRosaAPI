using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Carts.Exceptions;

public sealed class CartUserRequiredException()
    : DomainValidationException("cart.user.invalid", "Cart user is required.");

public sealed class CartItemsCollectionRequiredException()
    : DomainValidationException("cart.items.required", "Cart items collection is required.");

public sealed class CartItemRequiredException()
    : DomainValidationException("cart.item.required", "Cart item is required.");

public sealed class CartItemCartRequiredException()
    : DomainValidationException("cart_item.cart.invalid", "Cart item must be associated with a cart.");

public sealed class CartItemProductRequiredException()
    : DomainValidationException("cart_item.product.invalid", "Cart item product is required.");

public sealed class CartItemQuantityInvalidException()
    : DomainValidationException("cart_item.quantity.invalid", "Cart item quantity must be greater than zero.");

public sealed class CartItemUnitPriceRequiredException()
    : DomainValidationException("cart_item.unit_price.required", "Cart item unit price is required.");

public sealed class CartProductEligibilityServiceRequiredException()
    : DomainValidationException("cart.product_eligibility_service.required", "Cart product eligibility service is required.");
