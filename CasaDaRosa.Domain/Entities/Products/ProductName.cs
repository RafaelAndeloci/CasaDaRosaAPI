using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Products.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products;

public sealed class ProductName : ValueObject
{
    public string Value { get; }

    private ProductName(string value)
    {
        Value = value;
    }

    public static ProductName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ProductNameRequiredException();
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > 150)
        {
            throw new ProductNameTooLongException();
        }

        return new ProductName(normalizedValue);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
