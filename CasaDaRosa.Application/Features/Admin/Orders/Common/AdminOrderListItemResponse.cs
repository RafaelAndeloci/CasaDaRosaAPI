using CasaDaRosa.Application.Common.Responses;

namespace CasaDaRosa.Application.Features.Admin.Orders.Common;

public sealed record AdminOrderListItemResponse(
    Guid Id,
    Guid UserId,
    Guid AddressId,
    EnumValueResponse PaymentMethod,
    EnumValueResponse Status,
    DateTime DeliveryAvailableFromUtc,
    decimal TotalAmount,
    string? CurrencyCode,
    int ItemsCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
