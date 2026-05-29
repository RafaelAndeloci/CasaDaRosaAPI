using System.Net;
using System.Net.Http.Json;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDaRosa.API.E2ETests.Features.Auth;

public class AdminAuthEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AdminAuthEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAdmin_WithAuthenticatedAdmin_ShouldReturnCreated()
    {
        var currentAdmin = await SeedAdminAsync();
        Authenticate(currentAdmin.Id, currentAdmin.Email.ToString(), true);

        var response = await _client.PostAsJsonAsync("/api/auth/admins", new
        {
            fullName = "Novo Admin",
            email = $"novo-admin.{Guid.NewGuid():N}@example.com",
            password = "123456",
            phoneNumber = "+55 (16) 99999-9999"
        });

        var body = await response.ReadSuccessAsync<CreateAdminResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body.Should().NotBeNull();
        body!.Data.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task PromoteUserToAdmin_WithAuthenticatedAdmin_ShouldReturnNoContent()
    {
        var currentAdmin = await SeedAdminAsync();
        var targetUser = await SeedConfirmedUserAsync();
        Authenticate(currentAdmin.Id, currentAdmin.Email.ToString(), true);

        var response = await _client.PatchAsync($"/api/auth/users/{targetUser.Id}/promote-to-admin", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var updatedUser = await dbContext.Users.FindAsync(targetUser.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ShouldReturnNoContent()
    {
        var user = await SeedPendingUserAsync();

        var response = await _client.PostAsJsonAsync("/api/auth/confirm-email", new
        {
            email = user.Email.ToString(),
            token = user.EmailConfirmationToken
        });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var updatedUser = await dbContext.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public async Task ResendConfirmation_WithExistingUser_ShouldReturnNoContent()
    {
        var user = await SeedPendingUserAsync();
        var previousToken = user.EmailConfirmationToken;

        var response = await _client.PostAsJsonAsync("/api/auth/resend-confirmation", new
        {
            email = user.Email.ToString()
        });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var updatedUser = await dbContext.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.EmailConfirmationToken.Should().NotBe(previousToken);
    }

    private void Authenticate(Guid userId, string email, bool isAdmin)
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, email);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, isAdmin.ToString().ToLowerInvariant());
    }

    private async Task<User> SeedAdminAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var user = User.CreateAdmin(
            "Admin Master",
            $"admin.{Guid.NewGuid():N}@example.com",
            "HASH::123456",
            "+55 (16) 99999-9999");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    private async Task<User> SeedPendingUserAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var user = User.Create(
            "Cliente Silva",
            $"cliente.{Guid.NewGuid():N}@example.com",
            "HASH::123456",
            "+55 (16) 91234-5678");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    private async Task<User> SeedConfirmedUserAsync()
    {
        var user = await SeedPendingUserAsync();

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var trackedUser = await dbContext.Users.FindAsync(user.Id);
        trackedUser.Should().NotBeNull();
        trackedUser!.ConfirmEmail(trackedUser.EmailConfirmationToken);
        await dbContext.SaveChangesAsync();
        return trackedUser;
    }

    private sealed record CreateAdminResponse(Guid UserId);
}
