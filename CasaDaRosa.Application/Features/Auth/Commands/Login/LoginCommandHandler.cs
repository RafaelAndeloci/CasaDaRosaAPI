using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    ISecurityService securityService,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !securityService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedApplicationException("auth.invalid_credentials", "Invalid email or password.");
        }

        if (!user.CanAuthenticate())
        {
            throw new ForbiddenApplicationException("auth.email_not_confirmed", "Your email must be confirmed before login.");
        }

        var token = jwtTokenGenerator.GenerateToken(user.Id, user.Email.ToString(), [AuthUserRole.Customer]);

        return new LoginResponse(AuthResponseFactory.Create(user, token.AccessToken, token.ExpiresAtUtc));
    }
}
