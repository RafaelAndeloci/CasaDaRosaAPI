using System.Net;
using System.Net.Http.Json;
using CasaDaRosa.API.E2ETests.Contracts;
using CasaDaRosa.API.E2ETests.Infrastructure;
using CasaDaRosa.Domain.Entities.Addresses;
using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.Entities.Categories;
using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Domain.ValueObjects;
using CasaDaRosa.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDaRosa.API.E2ETests.Features.Orders;

public class OrderEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public OrderEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Checkout_ThenGetMyOrders_AndGetById_ShouldReturnOrderFlow()
    {
        var user = await SeedConfirmedUserAsync();
        var address = await SeedAddressAsync(user.Id);
        var product = await SeedProductAsync();
        await SeedCartAsync(user.Id, product);
        AuthenticateAsUser(user.Id, user.Email.ToString());

        var checkoutResponse = await _client.PostAsJsonAsync("/api/orders/checkout", new
        {
            addressId = address.Id,
            paymentMethodId = (int)PaymentMethod.Pix,
            deliveryAvailableFromUtc = DateTime.UtcNow.AddHours(4)
        });

        var checkoutBody = await checkoutResponse.ReadSuccessAsync<OrderResponse>();

        checkoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        checkoutBody.Should().NotBeNull();
        checkoutBody!.Data.AddressId.Should().Be(address.Id);
        checkoutBody.Data.Status.Id.Should().Be((int)OrderStatus.Confirmed);
        checkoutBody.Data.Items.Should().ContainSingle();

        var getOrdersResponse = await _client.GetAsync($"/api/orders?statusId={(int)OrderStatus.Confirmed}");
        var getOrdersBody = await getOrdersResponse.ReadSuccessAsync<PagedResult<OrderResponse>>();

        getOrdersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getOrdersBody.Should().NotBeNull();
        getOrdersBody!.Data.Items.Should().ContainSingle(order => order.Id == checkoutBody.Data.Id);

        var getByIdResponse = await _client.GetAsync($"/api/orders/{checkoutBody.Data.Id}");
        var getByIdBody = await getByIdResponse.ReadSuccessAsync<OrderResponse>();

        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getByIdBody.Should().NotBeNull();
        getByIdBody!.Data.Id.Should().Be(checkoutBody.Data.Id);
        getByIdBody.Data.TotalAmount.Should().Be(50m);
    }

    [Fact]
    public async Task Checkout_WithEmptyCart_ShouldReturnUnprocessableEntity()
    {
        var user = await SeedConfirmedUserAsync();
        var address = await SeedAddressAsync(user.Id);
        await SeedEmptyCartAsync(user.Id);
        AuthenticateAsUser(user.Id, user.Email.ToString());

        var response = await _client.PostAsJsonAsync("/api/orders/checkout", new
        {
            addressId = address.Id,
            paymentMethodId = (int)PaymentMethod.Pix,
            deliveryAvailableFromUtc = DateTime.UtcNow.AddHours(4)
        });

        var error = await response.ReadErrorAsync();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        error.Should().NotBeNull();
        error!.Code.Should().Be("orders.checkout.empty_cart");
    }

    [Fact]
    public async Task AdminOrders_ShouldGetList_GetById_AndUpdateStatus()
    {
        var user = await SeedConfirmedUserAsync();
        var address = await SeedAddressAsync(user.Id);
        var product = await SeedProductAsync();
        var order = await SeedConfirmedOrderAsync(user.Id, address.Id, product);
        AuthenticateAsAdmin();

        var getOrdersResponse = await _client.GetAsync($"/api/orders/admin?userId={user.Id}&statusId={(int)OrderStatus.Confirmed}&paymentMethodId={(int)PaymentMethod.Pix}");
        var getOrdersBody = await getOrdersResponse.ReadSuccessAsync<PagedResult<AdminOrderListItemResponse>>();

        getOrdersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getOrdersBody.Should().NotBeNull();
        getOrdersBody!.Data.Items.Should().ContainSingle(item => item.Id == order.Id);

        var getByIdResponse = await _client.GetAsync($"/api/orders/admin/{order.Id}");
        var getByIdBody = await getByIdResponse.ReadSuccessAsync<AdminOrderResponse>();

        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getByIdBody.Should().NotBeNull();
        getByIdBody!.Data.Id.Should().Be(order.Id);
        getByIdBody.Data.UserId.Should().Be(user.Id);

        var updateStatusResponse = await _client.PatchAsJsonAsync($"/api/orders/admin/{order.Id}/status", new
        {
            orderId = order.Id,
            statusId = (int)OrderStatus.InPreparation
        });

        var updateStatusBody = await updateStatusResponse.ReadSuccessAsync<AdminOrderResponse>();

        updateStatusResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateStatusBody.Should().NotBeNull();
        updateStatusBody!.Data.Status.Id.Should().Be((int)OrderStatus.InPreparation);
    }

    private void AuthenticateAsUser(Guid userId, string email)
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AdminHeader);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        _client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, email);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.AdminHeader, "false");
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

    private async Task<User> SeedConfirmedUserAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var user = User.Create(
            "Cliente Silva",
            $"cliente.{Guid.NewGuid():N}@example.com",
            "HASH::123456",
            "+55 (16) 91234-5678");

        user.ConfirmEmail(user.EmailConfirmationToken);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    private async Task<Address> SeedAddressAsync(Guid userId)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var address = Address.Create(userId, "Rua das Flores", 123, "Centro", "Ribeirão Preto", "SP", "14000-000", null, null, true);
        dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync();
        return address;
    }

    private async Task<Product> SeedProductAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var category = Category.Create($"Categoria Pedido {Guid.NewGuid():N}", "Categoria pedido", true);
        var product = Product.Create(category.Id, $"Produto Pedido {Guid.NewGuid():N}", "Produto pedido", new Money(25m, Currency.Brl), 10);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return product;
    }

    private async Task SeedCartAsync(Guid userId, Product product)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var cart = Cart.Create(userId, CartStatus.Active, [CartItem.Create(Guid.NewGuid(), product.Id, 2, new Money(25m, Currency.Brl))]);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedEmptyCartAsync(Guid userId)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var cart = Cart.Create(userId, CartStatus.Empty, []);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
    }

    private async Task<Order> SeedConfirmedOrderAsync(Guid userId, Guid addressId, Product product)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CasaDaRosaDbContext>();
        var order = Order.Create(userId, addressId, PaymentMethod.Pix, DateTime.UtcNow.AddHours(4));
        order.AddItem(OrderItem.Create(order.Id, product.Id, product.Name.ToString(), 2, new Money(25m, Currency.Brl)));
        order.Confirm();
        order.ClearDomainEvents();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();
        return order;
    }

    private sealed record OrderResponse(
        Guid Id,
        Guid AddressId,
        EnumValueResponse PaymentMethod,
        EnumValueResponse Status,
        DateTime DeliveryAvailableFromUtc,
        decimal TotalAmount,
        string? CurrencyCode,
        IReadOnlyCollection<OrderItemResponse> Items);

    private sealed record OrderItemResponse(
        Guid Id,
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal Total,
        string? CurrencyCode);

    private sealed record AdminOrderListItemResponse(
        Guid Id,
        Guid UserId,
        Guid AddressId,
        EnumValueResponse PaymentMethod,
        EnumValueResponse Status,
        DateTime DeliveryAvailableFromUtc,
        decimal TotalAmount,
        string? CurrencyCode,
        int ItemsCount,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);

    private sealed record AdminOrderResponse(
        Guid Id,
        Guid UserId,
        Guid AddressId,
        EnumValueResponse PaymentMethod,
        EnumValueResponse Status,
        DateTime DeliveryAvailableFromUtc,
        decimal TotalAmount,
        string? CurrencyCode,
        IReadOnlyCollection<OrderItemResponse> Items,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);
}
