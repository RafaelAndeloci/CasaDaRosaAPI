namespace CasaDaRosa.Application.Exceptions;

public sealed class NotFoundApplicationException(string code, string message)
    : ApplicationExceptionBase(code, message);
