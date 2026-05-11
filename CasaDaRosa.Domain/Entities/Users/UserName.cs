using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Users;

public record UserName
{
    public string FirstName { get; private set; }
    public string Surname { get; private set; }
    public string FullName { get; private set; }

    private UserName() { }

    public static UserName Create(string fullname)
    {
        var parts = fullname.Split(" ");

        return new()
        {
            FirstName = parts[0],
            FullName = fullname,
            Surname = fullname.Replace($"{parts[0]} ", ""),
        };
    }
    public string GetInitials()
    {
        var initials = new StringBuilder();
        var parts = FullName.Split(" ");
        foreach (var part in parts)
        {
            if (!string.IsNullOrEmpty(part))
            {
                initials.Append(part[0]);
            }
        }
        return initials.ToString().ToUpper();
    }

    public override string ToString()
    { 
        return FullName;
    }
}
