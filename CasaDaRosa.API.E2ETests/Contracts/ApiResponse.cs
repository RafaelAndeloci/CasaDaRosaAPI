namespace CasaDaRosa.API.E2ETests.Contracts;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T Data,
    string TraceId);
