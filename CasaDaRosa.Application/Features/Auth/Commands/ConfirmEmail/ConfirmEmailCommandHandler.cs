using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetTrackedByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new NotFoundApplicationException("auth.user_not_found", "User not found.");
        }

        user.ConfirmEmail(request.Token);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
