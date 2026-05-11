using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record AddressUfAbbreviation
{
    internal static readonly AddressUfAbbreviation None = new("");

    public static readonly AddressUfAbbreviation AC = new("AC");
    public static readonly AddressUfAbbreviation AL = new("AL");
    public static readonly AddressUfAbbreviation AP = new("AP");
    public static readonly AddressUfAbbreviation AM = new("AM");
    public static readonly AddressUfAbbreviation BA = new("BA");
    public static readonly AddressUfAbbreviation CE = new("CE");
    public static readonly AddressUfAbbreviation DF = new("DF");
    public static readonly AddressUfAbbreviation GO = new("GO");
    public static readonly AddressUfAbbreviation MA = new("MA");
    public static readonly AddressUfAbbreviation MT = new("MT");
    public static readonly AddressUfAbbreviation MS = new("MS");
    public static readonly AddressUfAbbreviation MG = new("MG");
    public static readonly AddressUfAbbreviation PA = new("PA");
    public static readonly AddressUfAbbreviation PB = new("PB");
    public static readonly AddressUfAbbreviation PR = new("PR");
    public static readonly AddressUfAbbreviation PE = new("PE");
    public static readonly AddressUfAbbreviation PI = new("PI");
    public static readonly AddressUfAbbreviation SP = new("SP");
    public static readonly AddressUfAbbreviation RJ = new("RJ");
    public static readonly AddressUfAbbreviation ES = new("ES");
    public static readonly AddressUfAbbreviation RN = new("RN");
    public static readonly AddressUfAbbreviation RS = new("RS");
    public static readonly AddressUfAbbreviation RO = new("RO");
    public static readonly AddressUfAbbreviation RR = new("RR");
    public static readonly AddressUfAbbreviation SC = new("SC");
    public static readonly AddressUfAbbreviation SE = new("SE");
    public static readonly AddressUfAbbreviation TO = new("TO");


    private AddressUfAbbreviation(string ufCode) => Code = ufCode;

    public string Code { get; init; }

    public static AddressUfAbbreviation FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ??
               throw new ApplicationException($"The Uf code is invalid: {code}");
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
}