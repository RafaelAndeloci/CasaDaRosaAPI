using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Carts.Commands.AddItemToCart;
using CasaDaRosa.Application.Features.Carts.Commands.ChangeCartItemQuantity;
using CasaDaRosa.Application.Features.Carts.Queries.GetMyCart;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.Entities.Carts.Services;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Carts;

public class GetMyCartQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorized()
    {
        var userContext = new FakeUserContext { IsAuthenticated = false };
        var cartRepository = new Mock<ICartRepository>();
        var handler = new GetMyCartQueryHandler(userContext, cartRepository.Object);

        var action = () => handler.Handle(new GetMyCartQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldReturnEmptyCart()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var handler = new GetMyCartQueryHandler(userContext, cartRepository.Object);

        var response = await handler.Handle(new GetMyCartQuery(), CancellationToken.None);

        response.Id.Should().BeNull();
        response.Items.Should().BeEmpty();
        response.TotalAmount.Should().Be(0m);
        cartRepository.Verify(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCartExists_ShouldReturnMappedCart()
    {
        var userId = Guid.NewGuid();
        var item = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 2, new Money(15m, Currency.Brl));
        var cart = Cart.Create(userId, CartStatus.Active, [item]);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var handler = new GetMyCartQueryHandler(userContext, cartRepository.Object);

        var response = await handler.Handle(new GetMyCartQuery(), CancellationToken.None);

        response.Id.Should().Be(cart.Id);
        response.Items.Should().ContainSingle();
        response.TotalAmount.Should().Be(30m);
        response.CurrencyCode.Should().Be("BRL");
        cartRepository.Verify(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AddItemToCartCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldCreateCartAddItemAndSave()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", "Desc", new Money(25m, Currency.Brl), 10);
        var productId = product.Id;
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var productRepository = new Mock<IProductRepository>();
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);
        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(productId, 2))
            .Returns(Result.Success());

        var handler = new AddItemToCartCommandHandler(userContext, cartRepository.Object, productRepository.Object, productEligibilityService.Object, unitOfWork.Object);

        var response = await handler.Handle(new AddItemToCartCommand(productId, 2), CancellationToken.None);

        response.Items.Should().ContainSingle();
        response.TotalAmount.Should().Be(50m);
        cartRepository.Verify(repository => repository.AddAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFound()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var productRepository = new Mock<IProductRepository>();
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new AddItemToCartCommandHandler(userContext, cartRepository.Object, productRepository.Object, productEligibilityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new AddItemToCartCommand(Guid.NewGuid(), 1), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenCartAlreadyContainsProduct_ShouldThrowConflict()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", "Desc", new Money(25m, Currency.Brl), 10);
        var productId = product.Id;
        var existingItem = CartItem.Create(Guid.NewGuid(), productId, 1, new Money(25m, Currency.Brl));
        var cart = Cart.Create(userId, CartStatus.Active, [existingItem]);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var productRepository = new Mock<IProductRepository>();
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(productId, 1))
            .Returns(Result.Success());
        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var handler = new AddItemToCartCommandHandler(userContext, cartRepository.Object, productRepository.Object, productEligibilityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new AddItemToCartCommand(productId, 1), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictApplicationException>();
    }
}

public class ChangeCartItemQuantityCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldThrowNotFound()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var handler = new ChangeCartItemQuantityCommandHandler(userContext, cartRepository.Object, productEligibilityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ChangeCartItemQuantityCommand(Guid.NewGuid(), 3), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenCartContainsProduct_ShouldUpdateQuantityAndSave()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var item = CartItem.Create(Guid.NewGuid(), productId, 1, new Money(20m, Currency.Brl));
        var cart = Cart.Create(userId, CartStatus.Active, [item]);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        productEligibilityService
            .Setup(service => service.ValidateProductEligibility(productId, 3))
            .Returns(Result.Success());

        var handler = new ChangeCartItemQuantityCommandHandler(userContext, cartRepository.Object, productEligibilityService.Object, unitOfWork.Object);

        var response = await handler.Handle(new ChangeCartItemQuantityCommand(productId, 3), CancellationToken.None);

        response.Items.Single().Quantity.Should().Be(3);
        response.TotalAmount.Should().Be(60m);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductIsNotInCart_ShouldThrowNotFound()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var anotherItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 1, new Money(20m, Currency.Brl));
        var cart = Cart.Create(userId, CartStatus.Active, [anotherItem]);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var productEligibilityService = new Mock<ICartProductEligibilityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var handler = new ChangeCartItemQuantityCommandHandler(userContext, cartRepository.Object, productEligibilityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ChangeCartItemQuantityCommand(productId, 3), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}
