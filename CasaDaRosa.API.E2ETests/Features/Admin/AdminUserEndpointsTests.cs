using System.Net;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDaRosa.API.E2ETests.Features.Admin;

public class AdminUserEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AdminUserEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        AuthenticateAsAdmin();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnPagedUsers()
    {
        var user = await SeedUserAsync();

        var response = await _client.GetAsync($"/api/users?search={Uri.EscapeDataString(user.Email.ToString())}");
        var body = await response.ReadSuccessAsync<PagedResult<AdminUserListItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Data.Items.Should().ContainSingle(item => item.Id == user.Id);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUserDetails()
    {
        var user = await SeedUserAsync();

        var response = await _client.GetAsync($"/api/users/{user.Id}");
        var body = await response.ReadSuccessAsync<AdminUserResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Data.Id.Should().Be(user.Id);
        body.Data.Email.Should().Be(user.Email.ToString());
    }

    [Fact]
    public async Task DeactivateUser_ThenActivateUser_ShouldChangeUserStatus()
    {
        var user = await SeedConfirmedUserAsync();

        var deactivateResponse = await _client.PatchAsync($"/api/users/{user.Id}/deactivate", null);
        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfterDeactivate = await _client.GetAsync($"/api/users/{user.Id}");
        var deactivateBody = await getAfterDeactivate.ReadSuccessAsync<AdminUserResponse>();

        getAfterDeactivate.StatusCode.Should().Be(HttpStatusCode.OK);
        deactivateBody.Should().NotBeNull();
        deactivateBody!.Data.Status.Id.Should().Be((int)UserStatus.Inactive);

        var activateResponse = await _client.PatchAsync($"/api/users/{user.Id}/activate", null);
        activateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfterActivate = await _client.GetAsync($"/api/users/{user.Id}");
        var activateBody = await getAfterActivate.ReadSuccessAsync<AdminUserResponse>();

        getAfterActivate.StatusCode.Should().Be(HttpStatusCode.OK);
        activateBody.Should().NotBeNull();
        activateBody!.Data.Status.Id.Should().Be((int)UserStatus.Active);
    }

    private void AuthenticateAsAdmin()
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "admin@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "true");
    }

    private async Task<User> SeedUserAsync()
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
        var user = await SeedUserAsync();

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var trackedUser = await dbContext.Users.FindAsync(user.Id);
        trackedUser.Should().NotBeNull();
        trackedUser!.ConfirmEmail(trackedUser.EmailConfirmationToken);
        await dbContext.SaveChangesAsync();
        return trackedUser;
    }

    private sealed record AdminUserListItemResponse(
        Guid Id,
        string FullName,
        string Email,
        string? PhoneNumber,
        EnumValueResponse Role,
        EnumValueResponse Status,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);

    private sealed record AdminUserResponse(
        Guid Id,
        string FullName,
        string Email,
        string? PhoneNumber,
        EnumValueResponse Role,
        EnumValueResponse Status,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        DateTime? EmailConfirmedAtUtc);
}
