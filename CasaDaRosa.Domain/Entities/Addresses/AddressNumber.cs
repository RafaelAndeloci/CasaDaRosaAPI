using CasaDaRosa.Domain.Entities.Addresses.Exceptions;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record AddressNumber
{
    public short Value { get; private set; }

    private AddressNumber() { }

    public static AddressNumber Create(short value)
    {
        if (value <= 0) throw new AddressNumberMustBeGreaterThanZeroException();
        if (value.ToString().Length > 9) throw new AddressNumberTooLargeException();
        return new AddressNumber() with { Value = value };
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
