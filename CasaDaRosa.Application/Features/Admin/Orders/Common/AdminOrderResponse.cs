using CasaDaRosa.Application.Common.Responses;
using CasaDaRosa.Application.Features.Orders.Common;

namespace CasaDaRosa.Application.Features.Admin.Orders.Common;

public sealed record AdminOrderResponse(
    Guid Id,
    Guid UserId,
    Guid AddressId,
    EnumValueResponse PaymentMethod,
    EnumValueResponse Status,
    DateTime DeliveryAvailableFromUtc,
    decimal TotalAmount,
    string? CurrencyCode,
    IReadOnlyCollection<OrderItemResponse> Items,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
