using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products;

public sealed class StockQuantity : ValueObject
{
    public int Value { get; }

    private StockQuantity(int value)
    {
        Value = value;
    }

    public static StockQuantity Create(int value)
    {
        if (value < 0)
        {
            throw new DomainValidationException("product.stock.invalid", "Product stock cannot be negative.");
        }

        return new StockQuantity(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
