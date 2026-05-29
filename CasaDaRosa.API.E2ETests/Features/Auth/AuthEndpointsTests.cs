using System.Net;
using System.Net.Http.Json;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using FluentAssertions;

namespace CasaDaRosa.API.E2ETests.Features.Auth;

public class AuthEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ThenLoginWithoutConfirmation_ShouldReturnForbidden()
    {
        var registerPayload = new
        {
            fullName = "Maria da Silva",
            email = $"maria.{Guid.NewGuid():N}@example.com",
            password = "123456",
            phoneNumber = "+55 (16) 91234-5678"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerPayload);
        var registerBody = await registerResponse.ReadSuccessAsync<RegisterResponse>();

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        registerBody.Should().NotBeNull();
        registerBody!.Data.UserId.Should().NotBeEmpty();

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = registerPayload.email,
            password = registerPayload.password
        });

        var error = await loginResponse.ReadErrorAsync();

        loginResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        error.Should().NotBeNull();
        error!.Code.Should().Be("auth.email_not_confirmed");
    }

    [Fact]
    public async Task Register_WithDuplicatedEmail_ShouldReturnConflict()
    {
        var email = $"duplicated.{Guid.NewGuid():N}@example.com";
        var payload = new
        {
            fullName = "Maria da Silva",
            email,
            password = "123456",
            phoneNumber = "+55 (16) 91234-5678"
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/auth/register", payload);
        var secondResponse = await _client.PostAsJsonAsync("/api/auth/register", payload);
        var error = await secondResponse.ReadErrorAsync();

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        error.Should().NotBeNull();
        error!.Code.Should().Be("auth.email_already_in_use");
    }
}
