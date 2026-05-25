namespace CasaDaRosa.Application.Exceptions;

public sealed class UnauthorizedApplicationException(string? code = null, string? message = null)
    : ApplicationExceptionBase(code ?? "auth.unauthorized", message ?? "Authenticated user context is required.");
