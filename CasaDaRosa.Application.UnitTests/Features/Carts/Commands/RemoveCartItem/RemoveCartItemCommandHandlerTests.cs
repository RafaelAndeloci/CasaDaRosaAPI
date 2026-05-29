using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Carts.Commands.RemoveCartItem;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Carts.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenItemExists_ShouldRemoveAndReturnCart()
    {
        var userId = Guid.NewGuid();
        var item = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 2, new Money(15m, Currency.Brl));
        var cart = Cart.Create(userId, CartStatus.Active, [item]);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var handler = new RemoveCartItemCommandHandler(userContext, cartRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(new RemoveCartItemCommand(item.Id), CancellationToken.None);

        response.Items.Should().BeEmpty();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorized()
    {
        var userContext = new FakeUserContext { IsAuthenticated = false };
        var cartRepository = new Mock<ICartRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new RemoveCartItemCommandHandler(userContext, cartRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new RemoveCartItemCommand(Guid.NewGuid()), CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldThrowNotFound()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var handler = new RemoveCartItemCommandHandler(userContext, cartRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new RemoveCartItemCommand(Guid.NewGuid()), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenItemDoesNotExistInCart_ShouldThrowNotFound()
    {
        var userId = Guid.NewGuid();
        var cart = Cart.Create(userId, CartStatus.Active, []);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var itemId = Guid.NewGuid();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var handler = new RemoveCartItemCommandHandler(userContext, cartRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new RemoveCartItemCommand(itemId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}
