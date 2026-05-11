using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Users;

public partial record PhoneNumber
{
    public string FormattedValue { get; private set; }
    public short RawValue { get; private set; }
    public short AreaCode { get; private set; } = PhoneNumberConstants.DefaultAreaCode;
    public short CountryCode { get; private set; } = PhoneNumberConstants.DefaultCountryCode;

    private PhoneNumber() { }

    public static PhoneNumber Create(string formattedValue)
    {
        if (!PhoneNumberRegex().IsMatch(formattedValue))
            throw new ArgumentException("Invalid phone number format. Expected format: +CC (AC) VALUE (e.g +55 16 9123-1234)", nameof(formattedValue));
        var parts = formattedValue.Split(new[] { ' ', '(', ')', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var countryCode = short.Parse(parts[0].TrimStart('+'));
        var areaCode = short.Parse(parts[1]);
        var value = short.Parse(parts[2] + parts[3]);
        return Create(countryCode, areaCode, value);
    }

    public static PhoneNumber Create(short countryCode, short areaCode, short value)
    {
        var formattedValue = $"+{countryCode:00} ({areaCode:00}) {value:0000-0000}";

        return new PhoneNumber
        {
            FormattedValue = formattedValue,
            RawValue = value,
            AreaCode = areaCode,
            CountryCode = countryCode
        };
    }

    public static string Format(short fullPhoneNumber)
    {
        var formatted = "+{0:00} ({1:00}) {2:0000-0000}";

        return string.Format(formatted, fullPhoneNumber);
    }

    [GeneratedRegex(@"^\+\d{2} \(\d{2}\) \d{4}-\d{4}$")]
    public static partial Regex PhoneNumberRegex();
}

public static class PhoneNumberConstants
{
    public const short FormattedLength = 15; // CC-AC-VALUE (e.g., 55-11-91234-5678)
    public const short RawLength = 8; // VALUE (e.g., 912345678)
    public const short AreaCodeLength = 2; // AC (e.g., 11)
    public const short CountryCodeLength = 2; // CC (e.g., 55)
    public const short DefaultCountryCode = 55; // Default country code for Brazil
    public const short DefaultAreaCode = 16; // Default area code for Ribeirão Preto
}