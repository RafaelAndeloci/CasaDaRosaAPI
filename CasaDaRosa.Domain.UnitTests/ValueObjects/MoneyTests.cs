using CasaDaRosa.Domain.ValueObjects;
using CasaDaRosa.Domain.ValueObjects.Exceptions;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Addition_WithSameCurrency_ShouldReturnSummedMoney()
    {
        var first = new Money(10m, Currency.Brl);
        var second = new Money(15m, Currency.Brl);

        var result = first + second;

        result.Amount.Should().Be(25m);
        result.Currency.Should().Be(Currency.Brl);
    }

    [Fact]
    public void Addition_WithDifferentCurrencies_ShouldThrow()
    {
        var first = new Money(10m, Currency.Brl);
        var second = new Money(15m, Currency.Usd);

        var action = () => _ = first + second;

        action.Should().Throw<CurrencyMismatchException>();
    }

    [Fact]
    public void Zero_WithSpecificCurrency_ShouldCreateZeroMoney()
    {
        var money = Money.Zero(Currency.Brl);

        money.Amount.Should().Be(0m);
        money.Currency.Should().Be(Currency.Brl);
    }
}
