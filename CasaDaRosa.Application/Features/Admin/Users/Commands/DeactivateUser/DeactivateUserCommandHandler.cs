using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Commands.DeactivateUser;

public sealed class DeactivateUserCommandHandler(
    IUserContext userContext,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeactivateUserCommand>
{
    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var user = await userRepository.GetTrackedByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundApplicationException("users.not_found", "User not found.");
        }

        user.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
