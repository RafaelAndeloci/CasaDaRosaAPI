using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Users;

public record Email
{
    public string Value { get; set; }
    private Email()
    {
        
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("Cannot create an email without a value.");
        if (!IsValidEmail(value)) throw new ArgumentException("Invalid email format.");
        return new Email
        {
            Value = value
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
}
