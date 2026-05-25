using System.Text.RegularExpressions;
using CasaDaRosa.Domain.Entities.Users.Exceptions;

namespace CasaDaRosa.Domain.Entities.Users;

public partial record PhoneNumber
{
    public string FormattedValue { get; private set; } = string.Empty;
    public int RawValue { get; private set; }
    public short AreaCode { get; private set; } = PhoneNumberConstants.DefaultAreaCode;
    public short CountryCode { get; private set; } = PhoneNumberConstants.DefaultCountryCode;

    private PhoneNumber() { }

    public static PhoneNumber Create(string formattedValue)
    {
        if (string.IsNullOrWhiteSpace(formattedValue))
        {
            throw new InvalidPhoneNumberFormatException();
        }

        var normalizedValue = Normalize(formattedValue);

        if (!PhoneNumberRegex().IsMatch(normalizedValue))
        {
            throw new InvalidPhoneNumberFormatException();
        }

        var parts = normalizedValue.Split(new[] { ' ', '-', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
        var countryCode = short.Parse(parts[0].TrimStart('+'));
        var areaCode = short.Parse(parts[1]);
        var value = int.Parse(parts[2] + parts[3]);

        return Create(countryCode, areaCode, value);
    }

    public static PhoneNumber Create(short countryCode, short areaCode, int value)
    {
        var digits = value.ToString();

        if (digits.Length is not (PhoneNumberConstants.RawLength or PhoneNumberConstants.MobileRawLength))
        {
            throw new InvalidPhoneNumberFormatException();
        }

        var firstBlockLength = digits.Length == PhoneNumberConstants.MobileRawLength ? 5 : 4;
        var formattedValue = $"+{countryCode:00} ({areaCode:00}) {digits[..firstBlockLength]}-{digits[firstBlockLength..]}";

        return new PhoneNumber
        {
            FormattedValue = formattedValue,
            RawValue = value,
            AreaCode = areaCode,
            CountryCode = countryCode
        };
    }

    public static string Format(int fullPhoneNumber)
    {
        return Create(PhoneNumberConstants.DefaultCountryCode, PhoneNumberConstants.DefaultAreaCode, fullPhoneNumber).FormattedValue;
    }

    private static string Normalize(string value)
    {
        var trimmedValue = value.Trim();
        var match = FlexiblePhoneNumberNormalizationRegex().Match(trimmedValue);

        if (!match.Success)
        {
            return trimmedValue;
        }

        var areaCode = match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value;

        return $"+{match.Groups[1].Value} ({areaCode}) {match.Groups[4].Value}-{match.Groups[5].Value}";
    }

    [GeneratedRegex(@"^\+\d{2} \(\d{2}\) \d{4,5}-\d{4}$")]
    public static partial Regex PhoneNumberRegex();

    private static Regex FlexiblePhoneNumberNormalizationRegex()
    {
        return new Regex(@"^\+(\d{2})\s(?:\((\d{2})\)|(\d{2}))\s(\d{4,5})-(\d{4})$", RegexOptions.Compiled);
    }

    public override string ToString() 
    { 
        return FormattedValue;
    }
}

public static class PhoneNumberConstants
{
    public const short FormattedLength = 17;
    public const short MobileFormattedLength = 18;
    public const short RawLength = 8;
    public const short MobileRawLength = 9;
    public const short AreaCodeLength = 2; // AC (e.g., 11)
    public const short CountryCodeLength = 2; // CC (e.g., 55)
    public const short DefaultCountryCode = 55; // Default country code for Brazil
    public const short DefaultAreaCode = 16; // Default area code for Ribeirão Preto
}