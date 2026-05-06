using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money Create(decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainValidationException("money.invalid", "Amount must be greater than zero.");
        }

        return new Money(decimal.Round(amount, 2, MidpointRounding.ToEven));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
    }

    public override string ToString()
    {
        return Amount.ToString("0.00");
    }
}
