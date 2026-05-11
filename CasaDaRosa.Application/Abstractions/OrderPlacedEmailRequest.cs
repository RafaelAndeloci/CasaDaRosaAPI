using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Application.Abstractions;

public sealed record OrderPlacedEmailRequest(
    Guid UserId,
    Guid OrderId,
    Money TotalAmount,
    DateTime DeliveryAvailableFromUtc);
