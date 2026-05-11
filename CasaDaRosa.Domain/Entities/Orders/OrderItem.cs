using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Orders;

public class OrderItem : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductNameSnapshot { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }

    private OrderItem()
    {
    }

    public OrderItem(Guid orderId, Guid productId, string productNameSnapshot, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty)
        {
            throw new DomainValidationException("order.item.order.invalid", "Order item must be associated with an order.");
        }

        if (productId == Guid.Empty)
        {
            throw new DomainValidationException("order.item.product.invalid", "Order item product is required.");
        }

        if (string.IsNullOrWhiteSpace(productNameSnapshot))
        {
            throw new DomainValidationException("order.item.product_name.invalid", "Order item product name is required.");
        }

        if (quantity <= 0)
        {
            throw new DomainValidationException("order.item.quantity.invalid", "Order item quantity must be greater than zero.");
        }

        if (unitPrice <= 0)
        {
            throw new DomainValidationException("order.item.unit_price.invalid", "Order item unit price must be greater than zero.");
        }

        OrderId = orderId;
        ProductId = productId;
        ProductNameSnapshot = productNameSnapshot.Trim();
        Quantity = quantity;
        UnitPrice = decimal.Round(unitPrice, 2, MidpointRounding.ToEven);
        Total = decimal.Round(quantity * UnitPrice, 2, MidpointRounding.ToEven);
    }
}
