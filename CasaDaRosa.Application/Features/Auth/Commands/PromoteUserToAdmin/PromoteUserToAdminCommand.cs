using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.PromoteUserToAdmin;

public sealed record PromoteUserToAdminCommand(Guid UserId) : IRequest;
