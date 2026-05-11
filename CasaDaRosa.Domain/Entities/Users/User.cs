using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Addresses;

namespace CasaDaRosa.Domain.Entities.Users;

public class User : AuditableEntity, IAggregateRoot
{
    public UserName Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.PendingConfirmation;

    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private User(
        UserName name, 
        Email email, 
        string passwordHash, 
        PhoneNumber? phoneNumber, 
        UserStatus status)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Status = status;
    }

    public static User Create(string fullName, string email, string password, string? phoneNumber = null)
    {
        var userName = UserName.Create(fullName);
        var userEmail = Users.Email.Create(email);
        var phone = phoneNumber != null ? Users.PhoneNumber.Create(phoneNumber) : null;

        return new (
            name: userName,
            email: userEmail,
            passwordHash: password,
            phoneNumber: phone,
            status: UserStatus.PendingConfirmation
        );
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
