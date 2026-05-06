namespace CasaDaRosa.Application.Features.Products.Queries.GetProducts;

public sealed record ProductListItemResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId);
