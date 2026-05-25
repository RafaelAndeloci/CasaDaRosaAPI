using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.PromoteUserToAdmin;

public sealed class PromoteUserToAdminCommandHandler(
    IUserContext userContext,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<PromoteUserToAdminCommand>
{
    public async Task Handle(PromoteUserToAdminCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var currentUser = await userRepository.GetByIdAsync(userContext.UserId.Value, cancellationToken);

        if (currentUser is null)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!currentUser.IsAdmin())
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var user = await userRepository.GetTrackedByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundApplicationException("users.not_found", "User not found.");
        }

        user.PromoteToAdmin();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
