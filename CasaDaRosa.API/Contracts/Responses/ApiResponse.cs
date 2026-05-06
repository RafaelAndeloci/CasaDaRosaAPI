namespace CasaDaRosa.API.Contracts.Responses;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T Data,
    string TraceId);
