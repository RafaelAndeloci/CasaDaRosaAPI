namespace CasaDaRosa.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, IEnumerable<string> roles);
}
