using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.ValueObjects;

public sealed class CategoryName : ValueObject
{
    public string Value { get; }

    private CategoryName(string value)
    {
        Value = value;
    }

    public static CategoryName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("category.name.invalid", "Category name is required.");
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > 120)
        {
            throw new DomainValidationException("category.name.invalid", "Category name must have a maximum of 120 characters.");
        }

        return new CategoryName(normalizedValue);
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
