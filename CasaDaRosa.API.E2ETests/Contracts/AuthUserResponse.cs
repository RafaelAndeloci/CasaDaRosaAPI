namespace CasaDaRosa.API.E2ETests.Contracts;

public sealed record AuthUserResponse(
    Guid Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    EnumValueResponse Role,
    EnumValueResponse Status);
