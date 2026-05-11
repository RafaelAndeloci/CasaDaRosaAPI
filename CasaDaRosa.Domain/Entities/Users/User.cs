using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Addresses;
using CasaDaRosa.Domain.Entities.Users.Exceptions;
using CasaDaRosa.Domain.Exceptions;

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
        Guid id,
        UserName name, 
        Email email, 
        string passwordHash, 
        PhoneNumber? phoneNumber, 
        UserStatus status) : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Status = status;
    }

    public static User Create(string fullName, string email, string password, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new UserPasswordRequiredException();
        }

        var userName = UserName.Create(fullName);
        var userEmail = Users.Email.Create(email);
        var phone = string.IsNullOrWhiteSpace(phoneNumber) ? null : Users.PhoneNumber.Create(phoneNumber);

        return new (
            id: Guid.NewGuid(),
            name: userName,
            email: userEmail,
            passwordHash: password,
            phoneNumber: phone,
            status: UserStatus.PendingConfirmation
        );
    }

    public void AssignAddress(Address address)
    {
        if (address is null)
        {
            throw new UserAddressRequiredException();
        }

        if(_addresses.Count == 5)
        {
            throw new UserAddressLimitExceededException();
        }

        if (_addresses.Any(existingAddress => existingAddress.Id == address.Id))
        {
            throw new UserAddressDuplicateException();
        }

        _addresses.Add(address);
        Touch();
    }
}
