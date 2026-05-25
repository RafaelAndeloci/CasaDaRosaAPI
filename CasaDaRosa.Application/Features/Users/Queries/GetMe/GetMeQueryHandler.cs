using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Responses;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Users.Queries.GetMe;

public sealed class GetMeQueryHandler(IUserContext userContext, IUserRepository userRepository) : IRequestHandler<GetMeQuery, UserProfileResponse?>
{
    public async Task<UserProfileResponse?> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var user = await userRepository.GetByIdAsync(userContext.UserId.Value, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new UserProfileResponse(
            user.Id,
            user.Name.ToString(),
            user.Email.ToString(),
            user.PhoneNumber?.ToString(),
            EnumValueResponse.FromEnum(user.Status));
    }
}
