using System.Net;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using FluentAssertions;

namespace CasaDaRosa.API.E2ETests.Features.Catalog;

public class CatalogEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CatalogEndpointsTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ShouldReturnOkWithPagedPayload()
    {
        var response = await _client.GetAsync("/api/categories");
        var body = await response.ReadSuccessAsync<PagedResult<CategoryListItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOkWithPagedPayload()
    {
        var response = await _client.GetAsync("/api/products");
        var body = await response.ReadSuccessAsync<PagedResult<ProductListItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data.Items.Should().NotBeNull();
    }
}
