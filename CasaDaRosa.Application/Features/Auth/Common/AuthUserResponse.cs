using CasaDaRosa.Application.Common.Responses;

namespace CasaDaRosa.Application.Features.Auth.Common;

public sealed record AuthUserResponse(
    Guid Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    EnumValueResponse Role,
    EnumValueResponse Status);
