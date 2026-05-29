using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Orders.Commands.UpdateOrderStatus;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Admin.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTransitionIsValid_ShouldUpdateOrderStatus()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var orderRepository = new Mock<IOrderRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Buquê", 1, new Money(20m, Currency.Brl)));

        orderRepository
            .Setup(repository => repository.GetTrackedByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new UpdateOrderStatusCommandHandler(userContext, orderRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(new UpdateOrderStatusCommand(order.Id, (int)OrderStatus.Confirmed), CancellationToken.None);

        response.Status.Id.Should().Be((int)OrderStatus.Confirmed);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var orderRepository = new Mock<IOrderRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var orderId = Guid.NewGuid();

        orderRepository
            .Setup(repository => repository.GetTrackedByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = new UpdateOrderStatusCommandHandler(userContext, orderRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new UpdateOrderStatusCommand(orderId, (int)OrderStatus.Confirmed), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenTransitionIsInvalid_ShouldThrowUnprocessable()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var orderRepository = new Mock<IOrderRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));

        orderRepository
            .Setup(repository => repository.GetTrackedByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new UpdateOrderStatusCommandHandler(userContext, orderRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new UpdateOrderStatusCommand(order.Id, (int)OrderStatus.OutForDelivery), CancellationToken.None);

        await action.Should().ThrowAsync<UnprocessableApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ShouldThrowForbidden()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = false };
        var orderRepository = new Mock<IOrderRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new UpdateOrderStatusCommandHandler(userContext, orderRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new UpdateOrderStatusCommand(Guid.NewGuid(), (int)OrderStatus.Confirmed), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }
}
