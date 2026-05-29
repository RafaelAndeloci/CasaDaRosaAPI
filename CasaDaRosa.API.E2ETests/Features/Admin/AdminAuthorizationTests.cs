using System.Net;
using System.Net.Http.Json;
using CasaDaRosa.API.E2ETests.Infrastructure;
using FluentAssertions;

namespace CasaDaRosa.API.E2ETests.Features.Admin;

public class AdminAuthorizationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AdminAuthorizationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/products", new
        {
            categoryId = Guid.NewGuid(),
            name = "Buquê Premium",
            description = "Especial",
            price = 49.90m,
            stockQuantity = 10,
            isActive = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProduct_WithAuthenticatedNonAdminUser_ShouldReturnForbidden()
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "customer@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");

        var response = await _client.PostAsJsonAsync("/api/products", new
        {
            categoryId = Guid.NewGuid(),
            name = "Buquê Premium",
            description = "Especial",
            price = 49.90m,
            stockQuantity = 10,
            isActive = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
