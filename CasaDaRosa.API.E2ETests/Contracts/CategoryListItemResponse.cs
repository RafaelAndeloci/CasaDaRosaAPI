namespace CasaDaRosa.API.E2ETests.Contracts;

public sealed record CategoryListItemResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive);
