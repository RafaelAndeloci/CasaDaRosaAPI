using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.CreateAdmin;

public sealed record CreateAdminCommand(
    string FullName,
    string Email,
    string Password,
    string? PhoneNumber) : IRequest<CreateAdminResponse>;
