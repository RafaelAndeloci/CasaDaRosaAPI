using CasaDaRosa.Application.Common.Responses;
using CasaDaRosa.Domain.Entities.Users;

namespace CasaDaRosa.Application.Features.Auth.Common;

public static class AuthResponseFactory
{
    public static AuthResponse Create(User user, string accessToken, DateTime expiresAtUtc)
    {
        return new AuthResponse(
            accessToken,
            "Bearer",
            expiresAtUtc,
            new AuthUserResponse(
                user.Id,
                user.Name.ToString(),
                user.Email.ToString(),
                user.PhoneNumber?.ToString(),
                EnumValueResponse.FromEnum(user.Role),
                EnumValueResponse.FromEnum(user.Status)));
    }
}
