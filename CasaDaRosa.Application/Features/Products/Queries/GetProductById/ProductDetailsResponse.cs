namespace CasaDaRosa.Application.Features.Products.Queries.GetProductById;

public sealed record ProductDetailsResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId,
    bool IsActive);
