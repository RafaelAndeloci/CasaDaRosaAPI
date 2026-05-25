namespace CasaDaRosa.Application.Features.Orders.Common;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Total,
    string? CurrencyCode);
