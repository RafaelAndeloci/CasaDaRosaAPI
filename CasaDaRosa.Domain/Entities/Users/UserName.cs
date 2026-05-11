using System.Text;
using CasaDaRosa.Domain.Entities.Users.Exceptions;

namespace CasaDaRosa.Domain.Entities.Users;

public record UserName
{
    public string FirstName { get; private set; }
    public string Surname { get; private set; }
    public string FullName { get; private set; }

    private UserName() { }

    public static UserName Create(string fullname)
    {
        if (string.IsNullOrWhiteSpace(fullname))
        {
            throw new UserNameRequiredException();
        }

        var normalizedFullName = fullname.Trim();
        var parts = normalizedFullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
        {
            throw new UserNameInvalidException();
        }

        return new()
        {
            FirstName = parts[0],
            FullName = normalizedFullName,
            Surname = string.Join(' ', parts.Skip(1)),
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
