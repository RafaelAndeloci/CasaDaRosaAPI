namespace CasaDaRosa.Application.Abstractions.Auth;

public sealed record SendEmailConfirmationRequest(
    Guid UserId,
    string FullName,
    string Email,
    string ConfirmationToken);
