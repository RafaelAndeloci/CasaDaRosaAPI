namespace CasaDaRosa.API.E2ETests.Contracts;

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    AuthUserResponse User);
