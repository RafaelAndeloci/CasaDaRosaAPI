using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Orders.Exceptions;
using CasaDaRosa.Domain.Entities.Orders.Events;
using CasaDaRosa.Domain.Exceptions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities.Orders;

public class Order : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid AddressId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime DeliveryAvailableFromUtc { get; private set; }
    public Money TotalAmount { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order(
        Guid id,
        Guid userId,
        Guid addressId,
        PaymentMethod paymentMethod,
        OrderStatus status,
        DateTime deliveryAvailableFromUtc,
        Money totalAmount,
        List<OrderItem> items) : base(id)
    {

        UserId = userId;
        AddressId = addressId;
        PaymentMethod = paymentMethod;
        Status = status;
        DeliveryAvailableFromUtc = deliveryAvailableFromUtc;
        TotalAmount = totalAmount;
        _items = items;
    }

    public static Order Create(
        Guid userId, 
        Guid addressId, 
        PaymentMethod paymentMethod, 
        DateTime deliveryAvailableFromUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new OrderUserRequiredException();
        }

        if (addressId == Guid.Empty)
        {
            throw new OrderAddressRequiredException();
        }

        if (deliveryAvailableFromUtc <= DateTime.UtcNow)
        {
            throw new OrderDeliveryWindowInvalidException();
        }

        return new(
            id: Guid.NewGuid(),
            userId: userId,
            addressId: addressId,
            paymentMethod: paymentMethod,
            status: OrderStatus.Pending,
            deliveryAvailableFromUtc: deliveryAvailableFromUtc,
            totalAmount: Money.Zero(),
            items: []);
    }

    public Result AddItem(OrderItem item)
    {
        if (item is null)
        {
            throw new OrderItemRequiredException();
        }

        _items.Add(item);
        RecalculateTotal();
        Touch();

        return Result.Success();
    }

    public Result Confirm()
    {
        if(Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.NotPending);
        }
        Status = OrderStatus.Confirmed;
        RaiseDomainEvent(new OrderPlacedDomainEvent(Id, UserId, TotalAmount, DeliveryAvailableFromUtc));
        Touch();
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Confirmed)
        {
            return Result.Failure(OrderErrors.NotConfirmed);
        }
        Status = OrderStatus.Cancelled;
        RaiseDomainEvent(new OrderCancelledDomainEvent(Id));
        Touch();
        return Result.Success();
    }

    private void RecalculateTotal()
    {
        var total = _items.Aggregate(Money.Zero(), (acc, item) => acc + item.Total);
        TotalAmount = total;
    }
}
