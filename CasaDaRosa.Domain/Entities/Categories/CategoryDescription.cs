using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Categories.Exceptions;

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
            throw new CategoryDescriptionEmptyException();
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > 500)
        {
            throw new CategoryDescriptionTooLongException();
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
