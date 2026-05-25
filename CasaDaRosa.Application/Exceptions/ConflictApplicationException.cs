namespace CasaDaRosa.Application.Exceptions;

public sealed class ConflictApplicationException(string code, string message)
    : ApplicationExceptionBase(code, message);
