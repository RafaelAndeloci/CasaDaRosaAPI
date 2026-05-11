using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities.Carts;

public class CartItem : AuditableEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }

    private CartItem(Guid id, Guid cartId, Guid productId, int quantity, Money unitPrice) : base(id)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static CartItem Create(Guid cartId, Guid productId, int quantity, Money unitPrice)
    {
        if (cartId == Guid.Empty)
        {
            throw new DomainValidationException("cart_item.cart.invalid", "Cart item must be associated with a cart.");
        }

        if (productId == Guid.Empty)
        {
            throw new DomainValidationException("cart_item.product.invalid", "Cart item product is required.");
        }

        if (quantity <= 0)
        {
            throw new DomainValidationException("cart_item.quantity.invalid", "Cart item quantity must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(unitPrice);

        return new CartItem(Guid.NewGuid(), cartId, productId, quantity, unitPrice);
    }
}
