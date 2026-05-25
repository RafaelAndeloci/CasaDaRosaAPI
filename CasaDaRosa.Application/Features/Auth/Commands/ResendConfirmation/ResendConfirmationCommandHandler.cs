using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Auth;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.ResendConfirmation;

public sealed class ResendConfirmationCommandHandler(
    IUserRepository userRepository,
    IAuthEmailService authEmailService,
    IUnitOfWork unitOfWork) : IRequestHandler<ResendConfirmationCommand>
{
    public async Task Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetTrackedByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new NotFoundApplicationException("auth.user_not_found", "User not found.");
        }

        user.RenewEmailConfirmation();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await authEmailService.SendEmailConfirmationAsync(
            new SendEmailConfirmationRequest(
                user.Id,
                user.Name.ToString(),
                user.Email.ToString(),
                user.EmailConfirmationToken),
            cancellationToken);
    }
}
