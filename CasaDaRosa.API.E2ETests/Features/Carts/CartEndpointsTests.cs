using System.Net;
using System.Net.Http.Json;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using CasaDaRosa.Domain.Entities.Categories;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.ValueObjects;
using CasaDaRosa.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDaRosa.API.E2ETests.Features.Carts;

public class CartEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CartEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddItemToCart_ShouldReturnUpdatedCart()
    {
        var userId = Guid.NewGuid();
        var productId = await SeedProductAsync();

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "customer@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");

        var addResponse = await _client.PostAsJsonAsync("/api/carts/items", new
        {
            productId,
            quantity = 2
        });

        var addBody = await addResponse.ReadSuccessAsync<CartResponse>();

        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        addBody.Should().NotBeNull();
        addBody!.Data.Items.Should().ContainSingle();
        addBody.Data.TotalAmount.Should().Be(50m);
        addBody.Data.Status.Id.Should().Be(2);
        addBody.Data.Items.Single().ProductId.Should().Be(productId);
        addBody.Data.Items.Single().Quantity.Should().Be(2);
    }

    [Fact]
    public async Task GetCart_AfterAddingItem_ShouldReturnOk()
    {
        var userId = Guid.NewGuid();
        var productId = await SeedProductAsync();

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "customer@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");

        await _client.PostAsJsonAsync("/api/carts/items", new
        {
            productId,
            quantity = 2
        });

        var getResponse = await _client.GetAsync("/api/carts");
        var getBody = await getResponse.ReadSuccessAsync<CartResponse>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getBody.Should().NotBeNull();
        getBody!.Data.Id.Should().NotBeNull();
        getBody.Data.Status.Id.Should().Be(2);
        getBody.Data.Items.Should().ContainSingle();
        getBody.Data.Items.Single().ProductId.Should().Be(productId);
        getBody.Data.Items.Single().Quantity.Should().Be(2);
        getBody.Data.TotalAmount.Should().Be(50m);
        getBody.Data.CurrencyCode.Should().Be("BRL");
    }

    [Fact]
    public async Task AddItemToCart_WithUnknownProduct_ShouldReturnNotFound()
    {
        var userId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, "customer@example.com");
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");

        var response = await _client.PostAsJsonAsync("/api/carts/items", new
        {
            productId = Guid.NewGuid(),
            quantity = 1
        });

        var error = await response.ReadErrorAsync();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        error.Should().NotBeNull();
        error!.Code.Should().Be("products.not_found");
    }

    private async Task<Guid> SeedProductAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var category = Category.Create($"Rosas {Guid.NewGuid():N}", "Flores", true);
        var product = Product.Create(category.Id, "Buquê Especial", "Com rosas", new Money(25m, Currency.Brl), 10);

        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product.Id;
    }

    private sealed record CartResponse(
        Guid? Id,
        EnumValueResponse Status,
        IReadOnlyCollection<CartItemResponse> Items,
        decimal TotalAmount,
        string? CurrencyCode);

    private sealed record CartItemResponse(
        Guid Id,
        Guid ProductId,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice,
        string? CurrencyCode);
}
