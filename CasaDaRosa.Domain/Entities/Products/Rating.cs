using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Products.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products;

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
            throw new ProductRatingOutOfRangeException();
        }

        return new Rating(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
