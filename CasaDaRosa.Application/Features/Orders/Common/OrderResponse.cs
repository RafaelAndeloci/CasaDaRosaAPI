using CasaDaRosa.Application.Common.Responses;

namespace CasaDaRosa.Application.Features.Orders.Common;

public sealed record OrderResponse(
    Guid Id,
    Guid AddressId,
    EnumValueResponse PaymentMethod,
    EnumValueResponse Status,
    DateTime DeliveryAvailableFromUtc,
    decimal TotalAmount,
    string? CurrencyCode,
    IReadOnlyCollection<OrderItemResponse> Items);
