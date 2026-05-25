using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Commands.ActivateUser;

public sealed record ActivateUserCommand(Guid UserId) : IRequest;
