using CasaDaRosa.Domain.Entities.Addresses.Exceptions;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record AddressUf
{
    public AddressUfAbbreviation Abbreviation { get; private set; } = AddressUfAbbreviation.None;
    public string FullName => Abbreviation.FullName;

    public static AddressUf Create(string abbreviated)
    {
        if (string.IsNullOrWhiteSpace(abbreviated))
        {
            throw new AddressUfRequiredException();
        }

        return new AddressUf
        {
            Abbreviation = AddressUfAbbreviation.FromCode(abbreviated)
        };
    }

    public override string ToString()
    {
        return FullName;
    }
}
