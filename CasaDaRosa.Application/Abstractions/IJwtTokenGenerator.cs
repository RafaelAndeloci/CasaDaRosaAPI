using CasaDaRosa.Application.Abstractions.Auth;

namespace CasaDaRosa.Application.Abstractions;

public interface IJwtTokenGenerator
{
    AuthTokenResult GenerateToken(Guid userId, string email, IEnumerable<string> roles);
}
