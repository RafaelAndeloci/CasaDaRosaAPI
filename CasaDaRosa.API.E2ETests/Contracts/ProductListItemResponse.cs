namespace CasaDaRosa.API.E2ETests.Contracts;

public sealed record ProductListItemResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId);
