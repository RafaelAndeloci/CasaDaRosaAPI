using CasaDaRosa.Application.Features.Admin.Users.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<AdminUserResponse>;
