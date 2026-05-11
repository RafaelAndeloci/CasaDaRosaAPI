using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record AddressUf
{
    public AddressUfAbbreviation Abbreviation { get; private set; }
    public string FullName { get; private set; } = string.Empty;

    public static AddressUf Create(string abbreviated, string fullName)
    {
        if (string.IsNullOrEmpty(abbreviated)) throw new ArgumentNullException("Cannot create a state without a abbreviation.");
        if (string.IsNullOrEmpty(fullName)) throw new ArgumentNullException("Cannot create a state without its full name.");

        return new AddressUf
        {
            Abbreviation = AddressUfAbbreviation.FromCode(abbreviated),
            FullName = fullName
        };
    }

    public override string ToString()
    {
        return FullName;
    }
}
