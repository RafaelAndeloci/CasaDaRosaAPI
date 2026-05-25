using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(Guid UserId) : IRequest;
