using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Addresses;

public partial record ZipCode
{
    /// <summary>
    /// Gets or sets the value formatted as a string for display or serialization purposes.
    /// Formatted to the pattern '00000-000'.
    /// </summary>
    public string FormattedValue { get; set; }

    /// <summary>
    /// Gets or sets the raw short value represented by this property.
    /// </summary>
    public short RawValue { get; set; }

    private ZipCode() { }


    /// <summary>
    /// Creates a new instance of the ZipCode value object from a formatted string representation.
    /// </summary>
    /// <remarks>The input string must match the Brazilian zip code format, consisting of five digits, a
    /// hyphen, and three digits (e.g., '12345-678').</remarks>
    /// <param name="formattedValue">The zip code in the format '00000-000'. Must not be null or empty.</param>
    /// <returns>A ZipCode instance representing the specified formatted value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formattedValue"/> is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="formattedValue"/> is not in the format '00000-000' or contains invalid characters.</exception>
    public static ZipCode Create(string formattedValue)
    {
        if(string.IsNullOrEmpty(formattedValue)) throw new ArgumentNullException("Cannot create a zip code without a value.");
        if(!Regex.IsMatch(formattedValue, @"^\d{5}-\d{3}$")) throw new ArgumentException("Zip code must be in the format '00000-000'.");         
            
        var rawValue = formattedValue.Replace("-", "");
        if (!short.TryParse(rawValue, out var parsedRawValue)) throw new ArgumentException("Zip code must contain only numbers and a hyphen.");

        return new ZipCode
        {
            FormattedValue = formattedValue,
            RawValue = parsedRawValue
        };
    }

    /// <summary>
    /// Creates a new instance of the ZipCode value object from a numeric value in the format '00000000'.
    /// </summary>
    /// <remarks>The input value is expected to represent a zip code without formatting. The method formats
    /// the value as '00000-000' before creating the ZipCode instance.</remarks>
    /// <param name="value">The numeric value representing the zip code. Must contain exactly 8 digits, corresponding to the format
    /// '00000-000'.</param>
    /// <returns>A ZipCode instance with the formatted and raw values set according to the specified zip code.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided value does not contain exactly 8 digits.</exception>
    public static ZipCode Create(short value)
    {
        if (value.ToString().Length != 8) throw new ArgumentException("Zip code must be in the format '00000-000'.");
        var formattedValue = $"{value.ToString().Substring(0, 5)}-{value.ToString().Substring(5, 3)}";
        return new ZipCode
        {
            FormattedValue = formattedValue,
            RawValue = value
        };
    }

    public static implicit operator ZipCode(string value)
    {
        return Create(value);
    }

    public static implicit operator ZipCode(short value)
    {
        return Create(value);
    }

    /// <summary>
    /// Formats a zip code represented as a short integer into the '00000-000' string format.
    /// </summary>
    /// <param name="value">The zip code value to format. Must contain exactly 8 digits when converted to a string.</param>
    /// <returns>A string representing the formatted zip code in the '00000-000' format.</returns>
    /// <exception cref="ArgumentException">Thrown if the string representation of value does not contain exactly 8 digits.</exception>
    public static string Format(short value)
    {
        if (value.ToString().Length != 8) throw new ArgumentException("Zip code must have 8 numbers in order to format to '00000-000'.");
        return $"{value.ToString().Substring(0, 5)}-{value.ToString().Substring(5, 3)}";
    }


    /// <summary>
    /// Formats a string as a Brazilian zip code (CEP) in the '00000-000' pattern.
    /// </summary>
    /// <remarks>If the input is already in the correct '00000-000' format, it is returned unchanged.
    /// Otherwise, the method inserts a hyphen after the fifth digit.</remarks>
    /// <param name="value">The zip code string to format. Must contain exactly 8 numeric digits.</param>
    /// <returns>A string representing the zip code in the '00000-000' format.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> contains more than 8 characters.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> does not contain only digits.</exception>
    public static string Format(string value)
    {
        if(ZipCodeFormatRegex().IsMatch(value)) return value;
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("Cannot format a zip code without a value.");
        if (value.Length > 8) throw new ArgumentException("Zip code must have 8 characters '00000-000'.");
        if (!RawZipCodeRegex().IsMatch(value)) throw new ArgumentException("Zip code must contain only numbers.");

        return $"{value.Substring(0, 5)}-{value.ToString().Substring(5, 3)}";
    }

    [GeneratedRegex(@"^\d{5}-\d{3}$")]
    public static partial Regex ZipCodeFormatRegex();
    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex RawZipCodeRegex();

    public override string ToString()
    {
        return FormattedValue;
    }
}
