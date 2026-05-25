using System.Globalization;
using System.Text;

namespace CasaDaRosa.Application.Common.Filters;

public static class TextFilterUtility
{
    public static bool ContainsNormalized(string? source, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            return false;
        }

        var normalizedSource = Normalize(source);
        var normalizedSearchTerm = Normalize(searchTerm);

        return normalizedSource.Contains(normalizedSearchTerm, StringComparison.Ordinal);
    }

    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        var previousWasWhitespace = false;

        foreach (var character in decomposed)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);

            if (unicodeCategory == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToUpperInvariant(character));
                previousWasWhitespace = false;
                continue;
            }

            if (char.IsWhiteSpace(character) && !previousWasWhitespace)
            {
                builder.Append(' ');
                previousWasWhitespace = true;
            }
        }

        return builder
            .ToString()
            .Trim()
            .Normalize(NormalizationForm.FormC);
    }
}
