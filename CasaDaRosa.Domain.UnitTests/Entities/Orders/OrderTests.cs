using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.Entities.Orders.Events;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.Entities.Orders;

public class OrderTests
{
    [Fact]
    public void AddItem_ShouldRecalculateTotal()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));
        var item = OrderItem.Create(order.Id, Guid.NewGuid(), "Buquê Especial", 2, new Money(15m, Currency.Brl));

        var result = order.AddItem(item);

        result.IsSuccess.Should().BeTrue();
        order.TotalAmount.Amount.Should().Be(30m);
        order.Items.Should().HaveCount(1);
    }

    [Fact]
    public void Confirm_ShouldSetConfirmedStatusAndRaiseDomainEvent()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Buquê Especial", 1, new Money(20m, Currency.Brl)));

        var result = order.Confirm();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is OrderPlacedDomainEvent);
    }

    [Fact]
    public void Confirm_WhenOrderIsNotPending_ShouldReturnFailure()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Buquê Especial", 1, new Money(20m, Currency.Brl)));
        order.Confirm();

        var result = order.Confirm();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotPending);
    }

    [Fact]
    public void Cancel_WhenConfirmed_ShouldCancelAndRaiseDomainEvent()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Buquê Especial", 1, new Money(20m, Currency.Brl)));
        order.Confirm();

        var result = order.Cancel();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().Contain(domainEvent => domainEvent is OrderCancelledDomainEvent);
    }

    [Fact]
    public void Cancel_WhenOrderIsNotConfirmed_ShouldReturnFailure()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotConfirmed);
    }

    [Fact]
    public void Create_WhenDeliveryWindowIsNotInFuture_ShouldThrow()
    {
        var action = () => Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddMinutes(-1));

        action.Should().Throw<CasaDaRosa.Domain.Entities.Orders.Exceptions.OrderDeliveryWindowInvalidException>();
    }

    [Fact]
    public void UpdateStatus_ShouldFollowHappyPathTransitionFlow()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Buquê Especial", 1, new Money(20m, Currency.Brl)));

        order.UpdateStatus(OrderStatus.Confirmed).IsSuccess.Should().BeTrue();
        order.UpdateStatus(OrderStatus.InPreparation).IsSuccess.Should().BeTrue();
        order.UpdateStatus(OrderStatus.OutForDelivery).IsSuccess.Should().BeTrue();
        var result = order.UpdateStatus(OrderStatus.Delivered);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void UpdateStatus_WhenTransitionIsInvalid_ShouldReturnFailure()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Pix, DateTime.UtcNow.AddHours(2));

        var result = order.UpdateStatus(OrderStatus.InPreparation);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidStatusTransition);
    }
}
