using CasaDaRosa.Domain.Abstractions;
using MediatR;

namespace CasaDaRosa.Application.Events;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
