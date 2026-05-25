using CasaDaRosa.Domain.ValueObjects.Exceptions;

namespace CasaDaRosa.Domain.ValueObjects;

public record Currency
{
    public static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    public static readonly Currency Brl = new("BRL");

    private Currency()
    {
        Code = string.Empty;
    }

    private Currency(string code) => Code = code;

    public string Code { get; private set; }

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ?? 
               throw new CurrencyCodeInvalidException(code);
    }

    public static Currency FromCodeOrNone(string? code)
    {
        return string.IsNullOrWhiteSpace(code)
            ? None
            : FromCode(code);
    }

    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Usd,
        Eur,
        Brl
    };
}