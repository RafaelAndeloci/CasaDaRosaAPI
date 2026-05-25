namespace CasaDaRosa.Application.Abstractions;

public interface ISecurityService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
