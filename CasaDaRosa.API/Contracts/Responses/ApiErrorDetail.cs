namespace CasaDaRosa.API.Contracts.Responses;

public sealed record ApiErrorDetail(
    string Code,
    string Message);
