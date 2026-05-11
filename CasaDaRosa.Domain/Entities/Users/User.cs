using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Addresses;

namespace CasaDaRosa.Domain.Entities.Users;

public class User : AuditableEntity, IAggregateRoot
{
    public UserName Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private User()
    {
    }

    public User(string name, string email, string passwordHash, string? phoneNumber)
    {
        Name = UserName.Create(name);
        Email = Email.Create(email);
        PasswordHash = passwordHash;
        if (phoneNumber != null)
        {
            PhoneNumber = PhoneNumber.Create(phoneNumber);
        }
     }

    public void AssignAddress(Address address)
    {
        if(_addresses.Count == 5)
        {
            throw new InvalidOperationException("A user cannot have more than 5 addresses.");
        }
        _addresses.Add(address);
    }
}
