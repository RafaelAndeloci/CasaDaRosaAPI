using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Categories.Common;
using CasaDaRosa.Domain.Entities.Categories;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, AdminCategoryResponse>
{
    public async Task<AdminCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var category = Category.Create(request.Name, request.Description, request.IsActive);

        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdminCategoryResponseMapper.ToResponse(category);
    }
}
