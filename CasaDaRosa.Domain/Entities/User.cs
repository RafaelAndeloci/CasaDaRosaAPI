using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities;

public class User : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private User()
    {
    }

    public User(string name, string email, string passwordHash, string? phoneNumber = null)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
    }
}
