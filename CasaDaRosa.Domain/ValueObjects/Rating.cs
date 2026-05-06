using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.ValueObjects;

public sealed class Rating : ValueObject
{
    public decimal Value { get; }

    private Rating(decimal value)
    {
        Value = value;
    }

    public static Rating Create(decimal value)
    {
        var isWithinRange = value >= 0m && value <= 5m;
        var isHalfStep = value * 2m == decimal.Truncate(value * 2m);

        if (!isWithinRange || !isHalfStep)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 0 and 5 in increments of 0.5.");
        }

        return new Rating(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
