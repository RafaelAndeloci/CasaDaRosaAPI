using System.Security.Claims;
using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Features.Auth.Common;
using Microsoft.AspNetCore.Http;

namespace CasaDaRosa.Infrastructure.Contexts;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? Email => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public bool IsAdmin => httpContextAccessor.HttpContext?.User.IsInRole(AuthUserRole.Admin) ?? false;

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
