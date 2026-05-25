using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Commands.ActivateUser;

public sealed class ActivateUserCommandHandler(
    IUserContext userContext,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActivateUserCommand>
{
    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
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

        user.Activate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
