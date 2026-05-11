using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Carts;

public class CartItem : AuditableEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private CartItem()
    {
    }

    public CartItem(Guid cartId, Guid productId, int quantity, decimal unitPrice)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
