using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.ValueObjects;

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
            throw new DomainValidationException("product.name.invalid", "Product name is required.");
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > 150)
        {
            throw new DomainValidationException("product.name.invalid", "Product name must have a maximum of 150 characters.");
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
