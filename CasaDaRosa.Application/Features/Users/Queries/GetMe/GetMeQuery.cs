using MediatR;

namespace CasaDaRosa.Application.Features.Users.Queries.GetMe;

public sealed record GetMeQuery() : IRequest<UserProfileResponse?>;
