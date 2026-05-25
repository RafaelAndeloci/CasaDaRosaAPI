namespace CasaDaRosa.Application.Features.Categories.Queries.GetCategories;

public sealed record CategoryListItemResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive);
