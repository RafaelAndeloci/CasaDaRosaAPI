using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Carts.Exceptions;
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
            throw new CartItemCartRequiredException();
        }

        if (productId == Guid.Empty)
        {
            throw new CartItemProductRequiredException();
        }

        if (quantity <= 0)
        {
            throw new CartItemQuantityInvalidException();
        }

        if (unitPrice is null)
        {
            throw new CartItemUnitPriceRequiredException();
        }

        return new CartItem(Guid.NewGuid(), cartId, productId, quantity, unitPrice);
    }

    public Result UpdateQuantity(int newQuantity)
    {
        if(newQuantity <= 0)
        {
            return Result.Failure(CartErrors.InvalidQuantity);
        }

        Quantity = newQuantity;
        Touch();
        return Result.Success();
    }
}
