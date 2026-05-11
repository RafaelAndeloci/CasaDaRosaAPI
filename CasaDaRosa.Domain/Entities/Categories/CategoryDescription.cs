using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Categories;

public sealed class CategoryDescription : ValueObject
{
    public string Value { get; }

    private CategoryDescription(string value)
    {
        Value = value;
    }

    public static CategoryDescription Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("category.description.invalid", "Category description cannot be empty.");
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > 500)
        {
            throw new DomainValidationException("category.description.invalid", "Category description must have a maximum of 500 characters.");
        }

        return new CategoryDescription(normalizedValue);
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
