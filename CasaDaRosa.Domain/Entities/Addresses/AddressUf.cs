using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record AddressUf
{
    public string AbbreviatedValue { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;


    public static readonly List<(AddressUfAbbreviation, AddressUf)> Common =
        [
            (AddressUfAbbreviation.SP, Create("SP", "São Paulo")),
            (AddressUfAbbreviation.RJ, Create("RJ", "Rio de Janeiro")),
            (AddressUfAbbreviation.MG, Create("MG", "Minas Gerais")),
            (AddressUfAbbreviation.ES, Create("ES", "Espírito Santo")),
            (AddressUfAbbreviation.PR, Create("PR", "Paraná")),
            (AddressUfAbbreviation.RS, Create("RS", "Rio Grande do Sul")),
            (AddressUfAbbreviation.SC, Create("SC", "Santa Catarina")),
            (AddressUfAbbreviation.BA, Create("BA", "Bahia")),
            (AddressUfAbbreviation.PE, Create("PE", "Pernambuco")),
            (AddressUfAbbreviation.CE, Create("CE", "Ceará"))
        ];

    public static AddressUf FromCommon(AddressUfAbbreviation abbreviation)
    {
        var common = Common.FirstOrDefault(x => x.Item1 == abbreviation);
        if (common == default) throw new ArgumentException($"The provided abbreviation '{abbreviation}' is not recognized as a common state.");
        return common.Item2;
    }

    public static AddressUf Create(string abbreviated, string fullName)
    {
        if (string.IsNullOrEmpty(abbreviated)) throw new ArgumentNullException("Cannot create a state without a abbreviation.");
        if (string.IsNullOrEmpty(fullName)) throw new ArgumentNullException("Cannot create a state without its full name.");

        return new AddressUf
        {
            AbbreviatedValue = abbreviated,
            FullName = fullName
        };
    }
}
