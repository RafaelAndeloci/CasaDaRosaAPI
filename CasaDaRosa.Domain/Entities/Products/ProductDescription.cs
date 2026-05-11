using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products;

public sealed class ProductDescription : ValueObject
{
    public string Value { get; }

    private ProductDescription(string value)
    {
        Value = value;
    }

    public static ProductDescription Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("product.description.invalid", "Product description cannot be empty.");
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > 1000)
        {
            throw new DomainValidationException("product.description.invalid", "Product description must have a maximum of 1000 characters.");
        }

        return new ProductDescription(normalizedValue);
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
