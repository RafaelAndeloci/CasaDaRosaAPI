using CasaDaRosa.Application.Common.Responses;

namespace CasaDaRosa.Application.Features.Admin.Users.Common;

public sealed record AdminUserListItemResponse(
    Guid Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    EnumValueResponse Role,
    EnumValueResponse Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
