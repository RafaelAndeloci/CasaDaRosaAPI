using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string? PhoneNumber) : IRequest<RegisterResponse>;
