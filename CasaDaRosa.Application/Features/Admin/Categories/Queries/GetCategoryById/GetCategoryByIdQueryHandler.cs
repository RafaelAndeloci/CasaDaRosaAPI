using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Categories.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryByIdQuery, AdminCategoryResponse>
{
    public async Task<AdminCategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundApplicationException("categories.not_found", "Category not found.");
        }

        return AdminCategoryResponseMapper.ToResponse(category);
    }
}
