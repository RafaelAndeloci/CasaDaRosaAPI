using System.Net;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDaRosa.API.E2ETests.Features.Users;

public class UserEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMe_WithAuthenticatedUser_ShouldReturnProfile()
    {
        var userId = Guid.NewGuid();
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var user = User.Create("Maria da Silva", "maria@example.com", "HASH::123456", "+55 (16) 91234-5678");

        typeof(Domain.Abstractions.Entity)
            .GetProperty(nameof(User.Id))!
            .SetValue(user, userId);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "maria@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");

        var response = await _client.GetAsync("/api/users/me");
        var body = await response.ReadSuccessAsync<AuthUserResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Data.Id.Should().Be(userId);
        body.Data.Email.Should().Be("maria@example.com");
        body.Data.FullName.Should().Be("Maria da Silva");
        body.Data.Role.Id.Should().Be((int)UserRole.Customer);
    }

    [Fact]
    public async Task GetMe_WhenAuthenticatedUserDoesNotExist_ShouldReturnNotFound()
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "missing@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");

        var response = await _client.GetAsync("/api/users/me");
        var error = await response.ReadErrorAsync();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        error.Should().NotBeNull();
        error!.Code.Should().Be("users.me.not_found");
    }
}
