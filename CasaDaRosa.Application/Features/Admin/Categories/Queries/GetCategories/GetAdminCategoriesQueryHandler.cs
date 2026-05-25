using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Filters;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Categories.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategories;

public sealed class GetAdminCategoriesQueryHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository) : IRequestHandler<GetAdminCategoriesQuery, PagedResult<AdminCategoryResponse>>
{
    public async Task<PagedResult<AdminCategoryResponse>> Handle(GetAdminCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var categories = await categoryRepository.GetAllAsync(cancellationToken);

        var filteredCategories = categories
            .Where(category => request.IsActive is null || category.IsActive == request.IsActive.Value)
            .Where(category => TextFilterUtility.ContainsNormalized(category.Name.ToString(), request.Name))
            .OrderBy(category => category.Name.ToString())
            .ToArray();

        var totalCount = filteredCategories.Length;

        var pagedCategories = filteredCategories
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(AdminCategoryResponseMapper.ToResponse)
            .ToArray();

        return PagedResult<AdminCategoryResponse>.Create(pagedCategories, request.PageNumber, request.PageSize, totalCount);
    }
}
