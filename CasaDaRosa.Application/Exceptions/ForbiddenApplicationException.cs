namespace CasaDaRosa.Application.Exceptions;

public sealed class ForbiddenApplicationException(string code, string message)
    : ApplicationExceptionBase(code, message);
