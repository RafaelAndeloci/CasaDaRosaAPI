using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Events;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrderById;
using CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrders;
using CasaDaRosa.Application.Features.Orders.Commands.CheckoutOrder;
using CasaDaRosa.Application.Features.Orders.Events.OrderPlaced;
using CasaDaRosa.Application.Features.Orders.Queries.GetMyOrders;
using CasaDaRosa.Application.Features.Orders.Queries.GetOrderById;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Addresses;
using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.Entities.Orders.Events;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Orders;

public class GetMyOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorized()
    {
        var userContext = new FakeUserContext { IsAuthenticated = false };
        var orderRepository = new Mock<IOrderRepository>();
        var handler = new GetMyOrdersQueryHandler(userContext, orderRepository.Object);

        var action = () => handler.Handle(new GetMyOrdersQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedApplicationException>();
    }

    [Fact]
    public async Task Handle_ShouldFilterAndPaginateOrders()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var orderRepository = new Mock<IOrderRepository>();
        var confirmedOrder = OrderHandlerTestsData.CreateConfirmedOrder(userId, Guid.NewGuid());
        var pendingOrder = Order.Create(userId, Guid.NewGuid(), PaymentMethod.Card, DateTime.UtcNow.AddHours(3));
        pendingOrder.AddItem(OrderItem.Create(pendingOrder.Id, Guid.NewGuid(), "Tulipa", 1, new Money(30m, Currency.Brl)));

        orderRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { confirmedOrder, pendingOrder });

        var handler = new GetMyOrdersQueryHandler(userContext, orderRepository.Object);

        var response = await handler.Handle(new GetMyOrdersQuery(StatusId: (int)OrderStatus.Confirmed, PageNumber: 1, PageSize: 10), CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().Id.Should().Be(confirmedOrder.Id);
        response.Items.Single().Status.Id.Should().Be((int)OrderStatus.Confirmed);
    }
}

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderBelongsToAnotherUser_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = Guid.NewGuid() };
        var order = OrderHandlerTestsData.CreateConfirmedOrder(Guid.NewGuid(), Guid.NewGuid());
        var orderRepository = new Mock<IOrderRepository>();

        orderRepository
            .Setup(repository => repository.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new GetOrderByIdQueryHandler(userContext, orderRepository.Object);

        var action = () => handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenOrderBelongsToUser_ShouldReturnMappedResponse()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var order = OrderHandlerTestsData.CreateConfirmedOrder(userId, Guid.NewGuid());
        var orderRepository = new Mock<IOrderRepository>();

        orderRepository
            .Setup(repository => repository.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new GetOrderByIdQueryHandler(userContext, orderRepository.Object);

        var response = await handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        response.Id.Should().Be(order.Id);
        response.TotalAmount.Should().Be(order.TotalAmount.Amount);
        response.Items.Should().HaveCount(1);
    }
}

public class CheckoutOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCartAndAddressAreValid_ShouldCreateOrderClearCartAndSave()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var address = Address.Create(userId, "Rua das Flores", 100, "Centro", "Ribeirão Preto", "SP", "14000-000", null, null, true);
        var cartItem = CartItem.Create(Guid.NewGuid(), productId, 2, new Money(25m, Currency.Brl));
        var cart = Cart.Create(userId, CartStatus.Active, [cartItem]);
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", "Desc", new Money(25m, Currency.Brl), 20);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        var productRepository = new Mock<IProductRepository>();
        var orderRepository = new Mock<IOrderRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        addressRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { address });
        productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new CheckoutOrderCommandHandler(
            userContext,
            cartRepository.Object,
            addressRepository.Object,
            productRepository.Object,
            orderRepository.Object,
            unitOfWork.Object);

        var deliveryDate = DateTime.UtcNow.AddHours(4);
        var response = await handler.Handle(new CheckoutOrderCommand(address.Id, (int)PaymentMethod.Pix, deliveryDate), CancellationToken.None);

        response.AddressId.Should().Be(address.Id);
        response.Status.Id.Should().Be((int)OrderStatus.Confirmed);
        response.TotalAmount.Should().Be(50m);
        cart.Items.Should().BeEmpty();
        cart.Status.Should().Be(CartStatus.Empty);
        orderRepository.Verify(repository => repository.AddAsync(
            It.Is<Order>(order =>
                order.UserId == userId
                && order.AddressId == address.Id
                && order.Status == OrderStatus.Confirmed
                && order.Items.Count == 1),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCartIsEmpty_ShouldThrowUnprocessable()
    {
        var userId = Guid.NewGuid();
        var cart = Cart.Create(userId, CartStatus.Empty, []);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        var productRepository = new Mock<IProductRepository>();
        var orderRepository = new Mock<IOrderRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var handler = new CheckoutOrderCommandHandler(
            userContext,
            cartRepository.Object,
            addressRepository.Object,
            productRepository.Object,
            orderRepository.Object,
            unitOfWork.Object);

        var action = () => handler.Handle(new CheckoutOrderCommand(Guid.NewGuid(), (int)PaymentMethod.Pix, DateTime.UtcNow.AddHours(4)), CancellationToken.None);

        await action.Should().ThrowAsync<UnprocessableApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFound()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var address = Address.Create(userId, "Rua das Flores", 100, "Centro", "Ribeirão Preto", "SP", "14000-000", null, null, true);
        var cartItem = CartItem.Create(Guid.NewGuid(), productId, 1, new Money(25m, Currency.Brl));
        var cart = Cart.Create(userId, CartStatus.Active, [cartItem]);
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var cartRepository = new Mock<ICartRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        var productRepository = new Mock<IProductRepository>();
        var orderRepository = new Mock<IOrderRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        cartRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        addressRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { address });
        productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new CheckoutOrderCommandHandler(
            userContext,
            cartRepository.Object,
            addressRepository.Object,
            productRepository.Object,
            orderRepository.Object,
            unitOfWork.Object);

        var action = () => handler.Handle(new CheckoutOrderCommand(address.Id, (int)PaymentMethod.Pix, DateTime.UtcNow.AddHours(4)), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}

public class GetAdminOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ShouldThrowForbidden()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = false };
        var orderRepository = new Mock<IOrderRepository>();
        var handler = new GetAdminOrdersQueryHandler(userContext, orderRepository.Object);

        var action = () => handler.Handle(new GetAdminOrdersQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_ShouldFilterAndPaginateOrders()
    {
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var orderRepository = new Mock<IOrderRepository>();
        var confirmedOrder = OrderHandlerTestsData.CreateConfirmedOrder(userId, Guid.NewGuid());
        var deliveredOrder = OrderHandlerTestsData.CreateConfirmedOrder(anotherUserId, Guid.NewGuid());
        deliveredOrder.UpdateStatus(OrderStatus.InPreparation);
        deliveredOrder.UpdateStatus(OrderStatus.OutForDelivery);
        deliveredOrder.UpdateStatus(OrderStatus.Delivered);

        orderRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { confirmedOrder, deliveredOrder });

        var handler = new GetAdminOrdersQueryHandler(userContext, orderRepository.Object);

        var response = await handler.Handle(
            new GetAdminOrdersQuery(UserId: userId, StatusId: (int)OrderStatus.Confirmed, PaymentMethodId: (int)PaymentMethod.Pix, PageNumber: 1, PageSize: 10),
            CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().UserId.Should().Be(userId);
    }
}

public class GetAdminOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ShouldThrowNotFound()
    {
        var orderId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var orderRepository = new Mock<IOrderRepository>();

        orderRepository
            .Setup(repository => repository.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = new GetAdminOrderByIdQueryHandler(userContext, orderRepository.Object);

        var action = () => handler.Handle(new GetAdminOrderByIdQuery(orderId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ShouldReturnDetailedResponse()
    {
        var order = OrderHandlerTestsData.CreateConfirmedOrder(Guid.NewGuid(), Guid.NewGuid());
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var orderRepository = new Mock<IOrderRepository>();

        orderRepository
            .Setup(repository => repository.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new GetAdminOrderByIdQueryHandler(userContext, orderRepository.Object);

        var response = await handler.Handle(new GetAdminOrderByIdQuery(order.Id), CancellationToken.None);

        response.Id.Should().Be(order.Id);
        response.UserId.Should().Be(order.UserId);
        response.Items.Should().HaveCount(1);
    }
}

public class SendOrderPlacedEmailWhenOrderPlacedDomainEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSendOrderPlacedEmailUsingDomainEventData()
    {
        var emailService = new Mock<IEmailService>();
        var handler = new SendOrderPlacedEmailWhenOrderPlacedDomainEventHandler(emailService.Object);
        var domainEvent = new OrderPlacedDomainEvent(Guid.NewGuid(), Guid.NewGuid(), new Money(75m, Currency.Brl), DateTime.UtcNow.AddHours(2));

        await handler.Handle(new DomainEventNotification<OrderPlacedDomainEvent>(domainEvent), CancellationToken.None);

        emailService.Verify(service => service.SendOrderPlacedAsync(
            It.Is<OrderPlacedEmailRequest>(request =>
                request.OrderId == domainEvent.OrderId
                && request.UserId == domainEvent.UserId
                && request.TotalAmount == domainEvent.TotalAmount
                && request.DeliveryAvailableFromUtc == domainEvent.DeliveryAvailableFromUtc),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

file static class OrderHandlerTestsData
{
    public static Order CreateConfirmedOrder(Guid userId, Guid addressId)
    {
        var order = Order.Create(userId, addressId, PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Buquê Especial", 1, new Money(20m, Currency.Brl)));
        order.Confirm();
        order.ClearDomainEvents();
        return order;
    }
}
