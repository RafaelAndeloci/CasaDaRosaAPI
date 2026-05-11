using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities.Orders.Events;

public sealed record OrderPlacedDomainEvent(
    Guid OrderId,
    Guid UserId,
    Money TotalAmount,
    DateTime DeliveryAvailableFromUtc) : DomainEvent;
