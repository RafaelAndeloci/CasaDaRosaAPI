using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Auth;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Common;
using CasaDaRosa.Domain.Entities.Users;
using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    ISecurityService securityService,
    IAuthEmailService authEmailService,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictApplicationException("auth.email_already_in_use", "A user with the provided email already exists.");
        }

        var passwordHash = securityService.HashPassword(request.Password);
        var user = User.Create(request.FullName, request.Email, passwordHash, request.PhoneNumber);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await authEmailService.SendEmailConfirmationAsync(
            new SendEmailConfirmationRequest(
                user.Id,
                user.Name.ToString(),
                user.Email.ToString(),
                user.EmailConfirmationToken),
            cancellationToken);

        return new RegisterResponse(user.Id, null);
    }
}
