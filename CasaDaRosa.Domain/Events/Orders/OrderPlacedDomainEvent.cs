using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Events.Orders;

public sealed record OrderPlacedDomainEvent(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount,
    DateTime DeliveryAvailableFromUtc) : DomainEvent;
