using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.ValueObjects.Exceptions;

namespace CasaDaRosa.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; private set; }
    public Currency? Currency { get; private set; }

    private Money()
    {
        Currency = ValueObjects.Currency.None;
    }

    public Money(decimal amount, Currency? currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new CurrencyMismatchException();
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new CurrencyMismatchException();
        }

        return new Money(first.Amount - second.Amount, first.Currency);
    }

    public static Money operator *(Money first, decimal multiplier)
    {
        return new Money(first.Amount * multiplier, first.Currency);
    }
    public static Money operator *(Money first, int multiplier)
    {
        return new Money(first.Amount * multiplier, first.Currency);
    }
    public static Money operator *(Money first, float multiplier)
    {
        return new Money(first.Amount * (decimal)multiplier, first.Currency);
    }
    public static Money operator *(Money first, double multiplier)
    {
        return new Money(first.Amount * (decimal)multiplier, first.Currency);
    }

    public static Money Zero () => new(0, Currency.None);
    public static Money Zero(Currency currency) => new (0, currency);
    public bool IsZero() => this == Zero();
}
