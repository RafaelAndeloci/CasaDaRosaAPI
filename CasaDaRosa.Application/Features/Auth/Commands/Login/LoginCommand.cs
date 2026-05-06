using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
