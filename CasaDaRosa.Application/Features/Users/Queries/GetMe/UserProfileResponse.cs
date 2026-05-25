using CasaDaRosa.Application.Common.Responses;

namespace CasaDaRosa.Application.Features.Users.Queries.GetMe;

public sealed record UserProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    EnumValueResponse Status);
