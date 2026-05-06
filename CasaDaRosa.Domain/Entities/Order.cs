using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Enums;
using CasaDaRosa.Domain.Events.Orders;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities;

public class Order : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid AddressId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime DeliveryAvailableFromUtc { get; private set; }
    public decimal TotalAmount { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order()
    {
    }

    public Order(Guid userId, Guid addressId, PaymentMethod paymentMethod, DateTime deliveryAvailableFromUtc, decimal totalAmount)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("order.user.invalid", "Order user is required.");
        }

        if (addressId == Guid.Empty)
        {
            throw new DomainValidationException("order.address.invalid", "Order address is required.");
        }

        if (deliveryAvailableFromUtc <= DateTime.UtcNow)
        {
            throw new DomainValidationException("order.delivery_window.invalid", "Delivery availability must be in the future.");
        }

        if (totalAmount <= 0)
        {
            throw new DomainValidationException("order.total.invalid", "Order total amount must be greater than zero.");
        }

        UserId = userId;
        AddressId = addressId;
        PaymentMethod = paymentMethod;
        DeliveryAvailableFromUtc = deliveryAvailableFromUtc;
        TotalAmount = decimal.Round(totalAmount, 2, MidpointRounding.ToEven);

        RaiseDomainEvent(new OrderPlacedDomainEvent(Id, UserId, TotalAmount, DeliveryAvailableFromUtc));
    }

    public void AddItem(Guid productId, string productNameSnapshot, int quantity, decimal unitPrice)
    {
        var item = new OrderItem(Id, productId, productNameSnapshot, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
        SetUpdatedAtUtc();
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        SetUpdatedAtUtc();
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
        SetUpdatedAtUtc();
    }

    private void RecalculateTotal()
    {
        TotalAmount = decimal.Round(_items.Sum(item => item.Total), 2, MidpointRounding.ToEven);
    }
}
