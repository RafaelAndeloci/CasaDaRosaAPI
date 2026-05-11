using CasaDaRosa.Domain.Entities.Addresses.Exceptions;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record AddressUfAbbreviation
{
    internal static readonly AddressUfAbbreviation None = new("", string.Empty);

    public static readonly AddressUfAbbreviation AC = new("AC", "Acre");
    public static readonly AddressUfAbbreviation AL = new("AL", "Alagoas");
    public static readonly AddressUfAbbreviation AP = new("AP", "Amapá");
    public static readonly AddressUfAbbreviation AM = new("AM", "Amazonas");
    public static readonly AddressUfAbbreviation BA = new("BA", "Bahia");
    public static readonly AddressUfAbbreviation CE = new("CE", "Ceará");
    public static readonly AddressUfAbbreviation DF = new("DF", "Distrito Federal");
    public static readonly AddressUfAbbreviation ES = new("ES", "Espírito Santo");
    public static readonly AddressUfAbbreviation GO = new("GO", "Goiás");
    public static readonly AddressUfAbbreviation MA = new("MA", "Maranhão");
    public static readonly AddressUfAbbreviation MT = new("MT", "Mato Grosso");
    public static readonly AddressUfAbbreviation MS = new("MS", "Mato Grosso do Sul");
    public static readonly AddressUfAbbreviation MG = new("MG", "Minas Gerais");
    public static readonly AddressUfAbbreviation PA = new("PA", "Pará");
    public static readonly AddressUfAbbreviation PB = new("PB", "Paraíba");
    public static readonly AddressUfAbbreviation PR = new("PR", "Paraná");
    public static readonly AddressUfAbbreviation PE = new("PE", "Pernambuco");
    public static readonly AddressUfAbbreviation PI = new("PI", "Piauí");
    public static readonly AddressUfAbbreviation RJ = new("RJ", "Rio de Janeiro");
    public static readonly AddressUfAbbreviation RN = new("RN", "Rio Grande do Norte");
    public static readonly AddressUfAbbreviation RS = new("RS", "Rio Grande do Sul");
    public static readonly AddressUfAbbreviation RO = new("RO", "Rondônia");
    public static readonly AddressUfAbbreviation RR = new("RR", "Roraima");
    public static readonly AddressUfAbbreviation SC = new("SC", "Santa Catarina");
    public static readonly AddressUfAbbreviation SP = new("SP", "São Paulo");
    public static readonly AddressUfAbbreviation SE = new("SE", "Sergipe");
    public static readonly AddressUfAbbreviation TO = new("TO", "Tocantins");


    private AddressUfAbbreviation(string ufCode, string fullName)
    {
        Code = ufCode;
        FullName = fullName;
    }

    public string Code { get; init; }
    public string FullName { get; init; }

    public static AddressUfAbbreviation FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new AddressUfRequiredException();
        }

        var normalizedCode = code.Trim().ToUpperInvariant();

        return All.FirstOrDefault(c => c.Code == normalizedCode) ??
               throw new AddressUfCodeInvalidException(code);
    }

    public static readonly IReadOnlyCollection<AddressUfAbbreviation> All = new[]
    {
        AC,
        AL,
        AP,
        AM,
        BA,
        CE,
        DF,
        GO,
        MA,
        MT,
        MS,
        MG,
        PA,
        PB,
        PR,
        PE,
        PI,
        SP,
        RJ,
        ES,
        RN,
        RS,
        RO,
        RR,
        SC,
        SE,
        TO,
    };

    public override string ToString()
    {
        return FullName;
    }
}