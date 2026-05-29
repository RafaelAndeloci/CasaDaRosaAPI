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

namespace CasaDaRosa.API.E2ETests.Features.Admin;

public class AdminManagementEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AdminManagementEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        AuthenticateAsAdmin();
    }

    [Fact]
    public async Task CreateCategory_ThenGetAdminCategoryById_ShouldReturnCreatedCategory()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/categories", new
        {
            name = $"Rosas {Guid.NewGuid():N}",
            description = "Categoria premium",
            isActive = true
        });

        var createBody = await createResponse.ReadSuccessAsync<AdminCategoryResponse>();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createBody.Should().NotBeNull();
        createBody!.Data.Id.Should().NotBeEmpty();
        createBody.Data.IsActive.Should().BeTrue();

        var getResponse = await _client.GetAsync($"/api/categories/admin/{createBody.Data.Id}");
        var getBody = await getResponse.ReadSuccessAsync<AdminCategoryResponse>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getBody.Should().NotBeNull();
        getBody!.Data.Id.Should().Be(createBody.Data.Id);
        getBody.Data.Description.Should().Be("Categoria premium");
    }

    [Fact]
    public async Task UpdateCategory_ThenDeactivateAndActivate_ShouldReflectStateChanges()
    {
        var categoryId = await SeedCategoryAsync();

        var updateResponse = await _client.PutAsJsonAsync($"/api/categories/{categoryId}", new
        {
            categoryId,
            name = $"Categoria Atualizada {Guid.NewGuid():N}",
            description = "Descrição atualizada",
            isActive = false
        });

        var updateBody = await updateResponse.ReadSuccessAsync<AdminCategoryResponse>();

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateBody.Should().NotBeNull();
        updateBody!.Data.IsActive.Should().BeFalse();

        var activateResponse = await _client.PatchAsync($"/api/categories/{categoryId}/activate", null);
        activateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/categories/admin/{categoryId}");
        var getBody = await getResponse.ReadSuccessAsync<AdminCategoryResponse>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getBody.Should().NotBeNull();
        getBody!.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateProduct_ThenGetAdminProductById_ShouldReturnCreatedProduct()
    {
        var categoryId = await SeedCategoryAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/products", new
        {
            categoryId,
            name = $"Buque {Guid.NewGuid():N}",
            description = "Especial",
            price = 49.90m,
            stockQuantity = 10,
            isActive = true
        });

        var createBody = await createResponse.ReadSuccessAsync<AdminProductResponse>();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createBody.Should().NotBeNull();
        createBody!.Data.Id.Should().NotBeEmpty();
        createBody.Data.CategoryId.Should().Be(categoryId);
        createBody.Data.StockQuantity.Should().Be(10);

        var getResponse = await _client.GetAsync($"/api/products/admin/{createBody.Data.Id}");
        var getBody = await getResponse.ReadSuccessAsync<AdminProductResponse>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getBody.Should().NotBeNull();
        getBody!.Data.Id.Should().Be(createBody.Data.Id);
        getBody.Data.Price.Should().Be(49.90m);
    }

    [Fact]
    public async Task UpdateProduct_ThenDeactivateAndActivate_ShouldReflectStateChanges()
    {
        var categoryId = await SeedCategoryAsync();
        var productId = await SeedProductAsync(categoryId);

        var updateResponse = await _client.PutAsJsonAsync($"/api/products/{productId}", new
        {
            productId,
            categoryId,
            name = $"Produto Atualizado {Guid.NewGuid():N}",
            description = "Nova descrição",
            price = 79.90m,
            stockQuantity = 3,
            isActive = false
        });

        var updateBody = await updateResponse.ReadSuccessAsync<AdminProductResponse>();

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateBody.Should().NotBeNull();
        updateBody!.Data.IsActive.Should().BeFalse();
        updateBody.Data.StockQuantity.Should().Be(3);

        var activateResponse = await _client.PatchAsync($"/api/products/{productId}/activate", null);
        activateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/products/admin/{productId}");
        var getBody = await getResponse.ReadSuccessAsync<AdminProductResponse>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getBody.Should().NotBeNull();
        getBody!.Data.IsActive.Should().BeTrue();
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

    private async Task<Guid> SeedCategoryAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var category = Category.Create($"Categoria {Guid.NewGuid():N}", "Categoria seed", true);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return category.Id;
    }

    private async Task<Guid> SeedProductAsync(Guid categoryId)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var product = Product.Create(categoryId, $"Produto {Guid.NewGuid():N}", "Produto seed", new Money(25m, Currency.Brl), 10);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return product.Id;
    }

    private sealed record AdminCategoryResponse(
        Guid Id,
        string Name,
        string? Description,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);

    private sealed record AdminProductResponse(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        int StockQuantity,
        Guid CategoryId,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);
}
