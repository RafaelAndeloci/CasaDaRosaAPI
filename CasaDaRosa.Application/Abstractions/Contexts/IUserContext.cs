namespace CasaDaRosa.Application.Abstractions.Contexts;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
