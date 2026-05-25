namespace CasaDaRosa.Application.Features.Admin.Categories.Common;

public sealed record AdminCategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
