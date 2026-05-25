using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities.Orders;

public class OrderItem : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductNameSnapshot { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money Total { get; private set; } = null!;

    private OrderItem() : base(Guid.Empty)
    {
    }

    private OrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        string productNameSnapshot,
        int quantity,
        Money unitPrice,
        Money total) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductNameSnapshot = productNameSnapshot;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Total = total;
    }

    public static OrderItem Create(Guid orderId, Guid productId, string productNameSnapshot, int quantity, Money unitPrice)
    {
        return new(
            id: Guid.NewGuid(),
            orderId: orderId,
            productId: productId,
            productNameSnapshot: productNameSnapshot,
            quantity: quantity,
            unitPrice: unitPrice,
            total: unitPrice * quantity);
    }
}
