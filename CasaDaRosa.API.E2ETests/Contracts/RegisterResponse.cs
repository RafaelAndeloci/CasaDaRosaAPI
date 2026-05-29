namespace CasaDaRosa.API.E2ETests.Contracts;

public sealed record RegisterResponse(Guid UserId, AuthResponse? Auth);
