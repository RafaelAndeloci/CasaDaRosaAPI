using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace CasaDaRosa.Application.Common.Responses;

public sealed record EnumValueResponse(int Id, string Description)
{
    public static EnumValueResponse FromEnum<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        return new EnumValueResponse(Convert.ToInt32(value), GetDescription(value));
    }

    private static string GetDescription<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        var member = typeof(TEnum)
            .GetMember(value.ToString())
            .FirstOrDefault();

        var descriptionAttribute = member?
            .GetCustomAttribute<DescriptionAttribute>();

        if (!string.IsNullOrWhiteSpace(descriptionAttribute?.Description))
        {
            return descriptionAttribute.Description;
        }

        return Humanize(value.ToString());
    }

    private static string Humanize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];

            if (index > 0 && char.IsUpper(character) && !char.IsWhiteSpace(value[index - 1]))
            {
                builder.Append(' ');
            }

            builder.Append(character);
        }

        return builder.ToString();
    }
}
