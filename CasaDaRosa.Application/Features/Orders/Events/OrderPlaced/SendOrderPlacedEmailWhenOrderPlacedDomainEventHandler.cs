using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Events;
using CasaDaRosa.Domain.Entities.Orders.Events;
using MediatR;

namespace CasaDaRosa.Application.Features.Orders.Events.OrderPlaced;

public sealed class SendOrderPlacedEmailWhenOrderPlacedDomainEventHandler(IEmailService emailService)
    : INotificationHandler<DomainEventNotification<OrderPlacedDomainEvent>>
{
    public Task Handle(DomainEventNotification<OrderPlacedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        return emailService.SendOrderPlacedAsync(
            new OrderPlacedEmailRequest(
                domainEvent.UserId,
                domainEvent.OrderId,
                domainEvent.TotalAmount,
                domainEvent.DeliveryAvailableFromUtc),
            cancellationToken);
    }
}
