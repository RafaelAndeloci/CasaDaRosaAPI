using CasaDaRosa.Application.Common.Responses;

namespace CasaDaRosa.Application.Features.Carts.Common;

public sealed record CartResponse(
    Guid? Id,
    EnumValueResponse Status,
    IReadOnlyCollection<CartItemResponse> Items,
    decimal TotalAmount,
    string? CurrencyCode);
