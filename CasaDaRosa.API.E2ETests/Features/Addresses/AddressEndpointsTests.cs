using System.Net;
using System.Net.Http.Json;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;
using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDaRosa.API.E2ETests.Features.Addresses;

public class AddressEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AddressEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAddress_ThenGetAddresses_ShouldReturnCreatedAddress()
    {
        var userId = Guid.NewGuid();
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var user = User.Create("Cliente Silva", "customer@example.com", "HASH::123456", "+55 (16) 91234-5678");

        typeof(Domain.Abstractions.Entity)
            .GetProperty(nameof(User.Id))!
            .SetValue(user, userId);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "customer@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");

        var createResponse = await _client.PostAsJsonAsync("/api/addresses", new
        {
            street = "Rua das Flores",
            number = 123,
            neighborhood = "Centro",
            city = "Ribeirão Preto",
            state = "SP",
            zipCode = "14000-000",
            complement = "Apto 12",
            reference = "Próximo à praça",
            isDefault = true
        });

        var createBody = await createResponse.ReadSuccessAsync<GuidResponse>();

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        createBody.Should().NotBeNull();
        createBody!.Data.Id.Should().NotBeEmpty();

        var getResponse = await _client.GetAsync("/api/addresses");
        var getBody = await getResponse.ReadSuccessAsync<PagedResult<AddressListItemResponse>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getBody.Should().NotBeNull();
        getBody!.Data.Items.Should().ContainSingle();
        getBody.Data.Items.Single().Street.Should().Be("Rua das Flores");
        getBody.Data.Items.Single().State.Should().Be("SP");
        getBody.Data.Items.Single().IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAddress_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", new
        {
            street = "Rua das Flores",
            number = 123,
            neighborhood = "Centro",
            city = "Ribeirão Preto",
            state = "SP",
            zipCode = "14000-000",
            complement = (string?)null,
            reference = (string?)null,
            isDefault = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private sealed record GuidResponse(Guid Id);
}
