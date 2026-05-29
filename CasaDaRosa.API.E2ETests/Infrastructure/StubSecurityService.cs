using CasaDaRosa.Application.Abstractions;

namespace CasaDaRosa.API.E2ETests.Infrastructure;

internal sealed class StubSecurityService : ISecurityService
{
    public string HashPassword(string password)
    {
        return $"HASH::{password}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return passwordHash == HashPassword(password);
    }
}
