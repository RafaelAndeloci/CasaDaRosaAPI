using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Categories.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, AdminCategoryResponse>
{
    public async Task<AdminCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var category = await categoryRepository.GetTrackedByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundApplicationException("categories.not_found", "Category not found.");
        }

        category.UpdateDetails(request.Name, request.Description);

        if (request.IsActive)
        {
            category.Activate();
        }
        else
        {
            category.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdminCategoryResponseMapper.ToResponse(category);
    }
}
