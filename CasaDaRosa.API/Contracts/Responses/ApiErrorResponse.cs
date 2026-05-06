namespace CasaDaRosa.API.Contracts.Responses;

public sealed record ApiErrorResponse(
    bool Success,
    string Code,
    string Message,
    IReadOnlyCollection<ApiErrorDetail> Errors,
    string TraceId);
