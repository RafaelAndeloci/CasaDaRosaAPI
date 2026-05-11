using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Orders.Events;

public sealed record OrderPlacedDomainEvent(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount,
    DateTime DeliveryAvailableFromUtc) : DomainEvent;
