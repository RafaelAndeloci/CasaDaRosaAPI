using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Domain.Abstractions;
using MediatR;

namespace CasaDaRosa.Application.Events;

public sealed class MediatRDomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent);

            if (notification is INotification mediatorNotification)
            {
                await publisher.Publish(mediatorNotification, cancellationToken);
            }
        }
    }
}
