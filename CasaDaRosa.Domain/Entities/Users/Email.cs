using CasaDaRosa.Domain.Entities.Users.Exceptions;

namespace CasaDaRosa.Domain.Entities.Users;

public record Email
{
    public string Value { get; set; }
    private Email()
    {
        
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new EmailRequiredException();

        var normalizedValue = value.Trim();

        if (!IsValidEmail(normalizedValue)) throw new InvalidEmailFormatException();

        return new Email
        {
            Value = normalizedValue
        };
    }

    public static bool IsValidEmail(string value)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            return addr.Address == value;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString()
    {
        return Value;
    }
}
