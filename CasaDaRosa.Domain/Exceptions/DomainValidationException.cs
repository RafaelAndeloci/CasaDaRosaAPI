namespace CasaDaRosa.Domain.Exceptions;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string code, string message, IReadOnlyCollection<string>? errors = null)
        : base(code, message)
    {
        Errors = errors ?? [];
    }

    public IReadOnlyCollection<string> Errors { get; }
}
