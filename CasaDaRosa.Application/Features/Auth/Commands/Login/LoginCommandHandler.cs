using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Authentication flow will be implemented in the next steps.");
    }
}
