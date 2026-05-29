using CasaDaRosa.Application.Abstractions.Contexts;

namespace CasaDaRosa.Application.UnitTests.TestDoubles;

internal sealed class FakeUserContext : IUserContext
{
    public Guid? UserId { get; init; }
    public string? Email { get; init; }
    public bool IsAdmin { get; init; }
    public bool IsAuthenticated { get; init; }
}
