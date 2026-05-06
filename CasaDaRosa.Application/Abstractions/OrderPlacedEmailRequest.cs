namespace CasaDaRosa.Application.Abstractions;

public sealed record OrderPlacedEmailRequest(
    Guid UserId,
    Guid OrderId,
    decimal TotalAmount,
    DateTime DeliveryAvailableFromUtc);
