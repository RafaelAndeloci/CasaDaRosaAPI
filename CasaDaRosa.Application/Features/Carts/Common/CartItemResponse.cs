namespace CasaDaRosa.Application.Features.Carts.Common;

public sealed record CartItemResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string? CurrencyCode);
