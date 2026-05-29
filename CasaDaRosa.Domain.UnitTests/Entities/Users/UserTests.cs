using CasaDaRosa.Domain.Entities.Addresses;
using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Domain.Entities.Users.Exceptions;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.Entities.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreatePendingCustomerWithConfirmationToken()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password", "+55 (16) 91234-5678");

        user.Role.Should().Be(UserRole.Customer);
        user.Status.Should().Be(UserStatus.PendingConfirmation);
        user.CanAuthenticate().Should().BeFalse();
        user.EmailConfirmationToken.Should().NotBeNullOrWhiteSpace();
        user.EmailConfirmationTokenExpiresAtUtc.Should().BeAfter(DateTime.UtcNow.AddHours(23));
        user.EmailConfirmedAtUtc.Should().BeNull();
        user.PhoneNumber.Should().NotBeNull();
    }

    [Fact]
    public void CreateAdmin_ShouldCreateActiveAdminWithoutConfirmationToken()
    {
        var user = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");

        user.Role.Should().Be(UserRole.Admin);
        user.Status.Should().Be(UserStatus.Active);
        user.CanAuthenticate().Should().BeTrue();
        user.IsAdmin().Should().BeTrue();
        user.EmailConfirmationToken.Should().BeEmpty();
        user.EmailConfirmationTokenExpiresAtUtc.Should().Be(DateTime.MinValue);
        user.EmailConfirmedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void ConfirmEmail_ShouldActivateUserAndClearToken()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var token = user.EmailConfirmationToken;

        user.ConfirmEmail(token);

        user.Status.Should().Be(UserStatus.Active);
        user.CanAuthenticate().Should().BeTrue();
        user.EmailConfirmationToken.Should().BeEmpty();
        user.EmailConfirmationTokenExpiresAtUtc.Should().Be(DateTime.MinValue);
        user.EmailConfirmedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void ConfirmEmail_WithInvalidToken_ShouldThrow()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");

        var action = () => user.ConfirmEmail("invalid-token");

        action.Should().Throw<UserEmailConfirmationTokenInvalidException>();
    }

    [Fact]
    public void RenewEmailConfirmation_ShouldGenerateANewToken()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var previousToken = user.EmailConfirmationToken;

        user.RenewEmailConfirmation();

        user.EmailConfirmationToken.Should().NotBe(previousToken);
        user.EmailConfirmationToken.Should().NotBeNullOrWhiteSpace();
        user.EmailConfirmationTokenExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void RenewEmailConfirmation_WhenUserIsActive_ShouldThrow()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        user.ConfirmEmail(user.EmailConfirmationToken);

        var action = () => user.RenewEmailConfirmation();

        action.Should().Throw<UserEmailAlreadyConfirmedException>();
    }

    [Fact]
    public void PromoteToAdmin_WhenAlreadyAdmin_ShouldThrow()
    {
        var user = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");

        var action = () => user.PromoteToAdmin();

        action.Should().Throw<UserRoleInvalidTransitionException>();
    }

    [Fact]
    public void Activate_WhenInactive_ShouldActivateAndKeepEmailConfirmedDate()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        user.ConfirmEmail(user.EmailConfirmationToken);
        var confirmedAt = user.EmailConfirmedAtUtc;
        user.Deactivate();

        user.Activate();

        user.Status.Should().Be(UserStatus.Active);
        user.EmailConfirmedAtUtc.Should().Be(confirmedAt);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrow()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        user.ConfirmEmail(user.EmailConfirmationToken);

        var action = () => user.Activate();

        action.Should().Throw<UserStatusInvalidTransitionException>();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldThrow()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        user.Deactivate();

        var action = () => user.Deactivate();

        action.Should().Throw<UserStatusInvalidTransitionException>();
    }

    [Fact]
    public void AssignAddress_WhenAddingSixthAddress_ShouldThrow()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");

        for (var index = 1; index <= 5; index++)
        {
            user.AssignAddress(CreateAddress(index));
        }

        var action = () => user.AssignAddress(CreateAddress(6));

        action.Should().Throw<UserAddressLimitExceededException>();
    }

    [Fact]
    public void AssignAddress_WhenAddingDuplicateAddress_ShouldThrow()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var address = CreateAddress(1);

        user.AssignAddress(address);

        var action = () => user.AssignAddress(address);

        action.Should().Throw<UserAddressDuplicateException>();
    }

    private static Address CreateAddress(int number)
    {
        return Address.Create(
            Guid.NewGuid(),
            $"Rua {number}",
            (short)number,
            "Centro",
            "Ribeirão Preto",
            "SP",
            "14000-000",
            null,
            null,
            false);
    }
}
