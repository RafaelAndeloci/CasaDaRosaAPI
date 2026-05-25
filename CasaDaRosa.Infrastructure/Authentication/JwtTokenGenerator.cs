using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CasaDaRosa.Infrastructure.Authentication;

public sealed class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public AuthTokenResult GenerateToken(Guid userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: SystemClock.UtcNow.AddMinutes(_options.ExpirationInMinutes),
            signingCredentials: signingCredentials);

        return new AuthTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            token.ValidTo);
    }
}
