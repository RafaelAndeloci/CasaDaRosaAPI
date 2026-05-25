namespace CasaDaRosa.Application.Abstractions.Auth;

public sealed record AuthTokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc);
