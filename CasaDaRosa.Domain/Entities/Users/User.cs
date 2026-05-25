using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Addresses;
using CasaDaRosa.Domain.Entities.Users.Exceptions;

namespace CasaDaRosa.Domain.Entities.Users;

public class User : AuditableEntity, IAggregateRoot
{
    private static readonly TimeSpan EmailConfirmationTokenLifetime = TimeSpan.FromHours(24);

    public UserName Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public PhoneNumber? PhoneNumber { get; private set; }
    public UserRole Role { get; private set; } = UserRole.Customer;
    public UserStatus Status { get; private set; } = UserStatus.PendingConfirmation;
    public string EmailConfirmationToken { get; private set; } = string.Empty;
    public DateTime EmailConfirmationTokenExpiresAtUtc { get; private set; }
    public DateTime? EmailConfirmedAtUtc { get; private set; }

    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private User() : base(Guid.Empty)
    {
    }

    private User(
        Guid id,
        UserName name, 
        Email email, 
        string passwordHash, 
        PhoneNumber? phoneNumber, 
        UserRole role,
        UserStatus status,
        string emailConfirmationToken,
        DateTime emailConfirmationTokenExpiresAtUtc,
        DateTime? emailConfirmedAtUtc) : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Role = role;
        Status = status;
        EmailConfirmationToken = emailConfirmationToken;
        EmailConfirmationTokenExpiresAtUtc = emailConfirmationTokenExpiresAtUtc;
        EmailConfirmedAtUtc = emailConfirmedAtUtc;
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
        var token = GenerateEmailConfirmationToken();
        var expiresAtUtc = DateTime.UtcNow.Add(EmailConfirmationTokenLifetime);

        return new (
            id: Guid.NewGuid(),
            name: userName,
            email: userEmail,
            passwordHash: password,
            phoneNumber: phone,
            role: UserRole.Customer,
            status: UserStatus.PendingConfirmation,
            emailConfirmationToken: token,
            emailConfirmationTokenExpiresAtUtc: expiresAtUtc,
            emailConfirmedAtUtc: null
        );
    }

    public static User CreateAdmin(string fullName, string email, string passwordHash, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new UserPasswordRequiredException();
        }

        var userName = UserName.Create(fullName);
        var userEmail = Users.Email.Create(email);
        var phone = string.IsNullOrWhiteSpace(phoneNumber) ? null : Users.PhoneNumber.Create(phoneNumber);

        return new(
            id: Guid.NewGuid(),
            name: userName,
            email: userEmail,
            passwordHash: passwordHash,
            phoneNumber: phone,
            role: UserRole.Admin,
            status: UserStatus.Active,
            emailConfirmationToken: string.Empty,
            emailConfirmationTokenExpiresAtUtc: DateTime.MinValue,
            emailConfirmedAtUtc: DateTime.UtcNow);
    }

    public bool CanAuthenticate()
    {
        return Status == UserStatus.Active;
    }

    public bool IsAdmin()
    {
        return Role == UserRole.Admin;
    }

    public void PromoteToAdmin()
    {
        if (Role == UserRole.Admin)
        {
            throw new UserRoleInvalidTransitionException();
        }

        Role = UserRole.Admin;
        Touch();
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
        {
            throw new UserStatusInvalidTransitionException();
        }

        Status = UserStatus.Active;

        if (EmailConfirmedAtUtc is null)
        {
            EmailConfirmedAtUtc = DateTime.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(EmailConfirmationToken))
        {
            EmailConfirmationToken = string.Empty;
            EmailConfirmationTokenExpiresAtUtc = DateTime.MinValue;
        }

        Touch();
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
        {
            throw new UserStatusInvalidTransitionException();
        }

        Status = UserStatus.Inactive;
        Touch();
    }

    public void ConfirmEmail(string token)
    {
        if (Status == UserStatus.Active)
        {
            throw new UserEmailAlreadyConfirmedException();
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UserEmailConfirmationTokenRequiredException();
        }

        if (!string.Equals(EmailConfirmationToken, token.Trim(), StringComparison.Ordinal))
        {
            throw new UserEmailConfirmationTokenInvalidException();
        }

        if (EmailConfirmationTokenExpiresAtUtc < DateTime.UtcNow)
        {
            throw new UserEmailConfirmationTokenExpiredException();
        }

        Status = UserStatus.Active;
        EmailConfirmedAtUtc = DateTime.UtcNow;
        EmailConfirmationToken = string.Empty;
        EmailConfirmationTokenExpiresAtUtc = DateTime.MinValue;
        Touch();
    }

    public void RenewEmailConfirmation()
    {
        if (Status == UserStatus.Active)
        {
            throw new UserEmailAlreadyConfirmedException();
        }

        EmailConfirmationToken = GenerateEmailConfirmationToken();
        EmailConfirmationTokenExpiresAtUtc = DateTime.UtcNow.Add(EmailConfirmationTokenLifetime);
        Touch();
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

    private static string GenerateEmailConfirmationToken()
    {
        return Convert.ToHexString(Guid.NewGuid().ToByteArray());
    }
}
