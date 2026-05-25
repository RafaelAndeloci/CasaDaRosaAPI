using CasaDaRosa.Application.Common.Responses;
using CasaDaRosa.Domain.Entities.Users;

namespace CasaDaRosa.Application.Features.Admin.Users.Common;

public static class AdminUserResponseMapper
{
    public static AdminUserResponse ToResponse(User user)
    {
        return new AdminUserResponse(
            user.Id,
            user.Name.ToString(),
            user.Email.ToString(),
            user.PhoneNumber?.ToString(),
            EnumValueResponse.FromEnum(user.Role),
            EnumValueResponse.FromEnum(user.Status),
            user.CreatedAtUtc,
            user.UpdatedAtUtc,
            user.EmailConfirmedAtUtc);
    }

    public static AdminUserListItemResponse ToListItem(User user)
    {
        return new AdminUserListItemResponse(
            user.Id,
            user.Name.ToString(),
            user.Email.ToString(),
            user.PhoneNumber?.ToString(),
            EnumValueResponse.FromEnum(user.Role),
            EnumValueResponse.FromEnum(user.Status),
            user.CreatedAtUtc,
            user.UpdatedAtUtc);
    }
}
