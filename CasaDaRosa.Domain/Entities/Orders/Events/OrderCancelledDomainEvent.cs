using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Orders.Events;

public record OrderCancelledDomainEvent(Guid OrderId) : DomainEvent;