using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.Entities.Carts.Services;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Domain.UnitTests.Entities.Carts;

public class CartTests
{
    [Fact]
    public void AddItem_WhenCartIsEmpty_ShouldAddItemAndActivateCart()
    {
        var cart = Cart.Create(Guid.NewGuid(), CartStatus.Empty, []);
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var item = CartItem.Create(cart.Id, Guid.NewGuid(), 2, new Money(15m, Currency.Brl));

        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(item.ProductId, item.Quantity))
            .Returns(Result.Success());

        var result = cart.AddItem(item, productEligibilityService.Object);

        result.IsSuccess.Should().BeTrue();
        cart.Status.Should().Be(CartStatus.Active);
        cart.Items.Should().ContainSingle();
    }

    [Fact]
    public void AddItem_WhenProductAlreadyExists_ShouldReturnFailure()
    {
        var cart = Cart.Create(Guid.NewGuid(), CartStatus.Active, []);
        var productId = Guid.NewGuid();
        var existingItem = CartItem.Create(cart.Id, productId, 1, new Money(10m, Currency.Brl));
        var newItem = CartItem.Create(cart.Id, productId, 2, new Money(10m, Currency.Brl));
        var productEligibilityService = new Mock<ICartProductEligibilityService>();

        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(existingItem.ProductId, existingItem.Quantity))
            .Returns(Result.Success());

        cart.AddItem(existingItem, productEligibilityService.Object);

        var result = cart.AddItem(newItem, productEligibilityService.Object);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.DuplicatedItem);
        cart.Items.Should().ContainSingle();
    }

    [Fact]
    public void RemoveItem_WhenItemExists_ShouldRemoveItemAndSetEmptyStatus()
    {
        var cart = Cart.Create(Guid.NewGuid(), CartStatus.Active, []);
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var item = CartItem.Create(cart.Id, Guid.NewGuid(), 1, new Money(20m, Currency.Brl));

        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(item.ProductId, item.Quantity))
            .Returns(Result.Success());

        cart.AddItem(item, productEligibilityService.Object);

        var result = cart.RemoveItem(item.Id);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
        cart.Status.Should().Be(CartStatus.Empty);
    }

    [Fact]
    public void ChangeProductQuantity_WhenProductIsNotInCart_ShouldReturnFailure()
    {
        var cart = Cart.Create(Guid.NewGuid(), CartStatus.Active, []);
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var existingItem = CartItem.Create(cart.Id, Guid.NewGuid(), 1, new Money(20m, Currency.Brl));

        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(existingItem.ProductId, existingItem.Quantity))
            .Returns(Result.Success());

        cart.AddItem(existingItem, productEligibilityService.Object);

        var result = cart.ChangeProductQuantity(Guid.NewGuid(), 3, productEligibilityService.Object);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.ProductNotInCart);
    }

    [Fact]
    public void ClearItems_WhenCartIsActive_ShouldRemoveAllItems()
    {
        var cart = Cart.Create(Guid.NewGuid(), CartStatus.Active, []);
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var item = CartItem.Create(cart.Id, Guid.NewGuid(), 1, new Money(20m, Currency.Brl));

        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(item.ProductId, item.Quantity))
            .Returns(Result.Success());

        cart.AddItem(item, productEligibilityService.Object);

        var result = cart.ClearItems();

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
        cart.Status.Should().Be(CartStatus.Empty);
    }

    [Fact]
    public void Abandon_WhenCartIsAlreadyAbandoned_ShouldReturnFailure()
    {
        var cart = Cart.Create(Guid.NewGuid(), CartStatus.Active, []);
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var item = CartItem.Create(cart.Id, Guid.NewGuid(), 1, new Money(20m, Currency.Brl));

        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(item.ProductId, item.Quantity))
            .Returns(Result.Success());

        cart.AddItem(item, productEligibilityService.Object);
        cart.Abandon();

        var result = cart.Abandon();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.AlreadyAbandoned);
    }
}
