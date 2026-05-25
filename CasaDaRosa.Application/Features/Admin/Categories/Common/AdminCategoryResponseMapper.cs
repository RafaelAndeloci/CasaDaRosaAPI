using CasaDaRosa.Domain.Entities.Categories;

namespace CasaDaRosa.Application.Features.Admin.Categories.Common;

public static class AdminCategoryResponseMapper
{
    public static AdminCategoryResponse ToResponse(Category category)
    {
        return new AdminCategoryResponse(
            category.Id,
            category.Name.ToString(),
            category.Description?.ToString(),
            category.IsActive,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);
    }
}
