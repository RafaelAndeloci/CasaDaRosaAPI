namespace CasaDaRosa.Application.Exceptions;

public sealed class UnprocessableApplicationException(string code, string message)
    : ApplicationExceptionBase(code, message);
