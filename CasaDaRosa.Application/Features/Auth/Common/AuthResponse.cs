using CasaDaRosa.Application.Common.Responses;

namespace CasaDaRosa.Application.Features.Auth.Common;

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    AuthUserResponse User);
