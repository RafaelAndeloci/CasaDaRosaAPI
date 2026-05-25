using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.ActivateCategory;

public sealed class ActivateCategoryCommandHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActivateCategoryCommand>
{
    public async Task Handle(ActivateCategoryCommand request, CancellationToken cancellationToken)
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

        category.Activate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
