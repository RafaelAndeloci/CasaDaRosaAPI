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

    public static Result<StockQuantity> Create(int value)
    {
        if (value < 0)
        {
            return Result.Failure<StockQuantity>(ProductErrors.InvalidStockQuantity);
        }

        return Result.Success(new StockQuantity(value));
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
