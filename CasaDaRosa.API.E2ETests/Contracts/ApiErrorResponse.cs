namespace CasaDaRosa.API.E2ETests.Contracts;

public sealed record ApiErrorResponse(
    bool Success,
    string Code,
    string Message,
    IReadOnlyCollection<ApiErrorDetail> Errors,
    string TraceId);
