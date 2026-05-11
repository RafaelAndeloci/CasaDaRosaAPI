using CasaDaRosa.Domain.Entities.Addresses.Exceptions;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record Street
{
    public string Value { get; private set; }
    private Street() { }

    public static Street Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new StreetRequiredException();
        return new Street() with { Value = value.Trim() };
    }

    public override string ToString()
    {
        return Value;
    }
}
