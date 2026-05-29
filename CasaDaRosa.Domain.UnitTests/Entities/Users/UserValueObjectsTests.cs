using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Domain.Entities.Users.Exceptions;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.Entities.Users;

public class UserNameTests
{
    [Fact]
    public void Create_WithValidFullName_ShouldSplitNameAndSurname()
    {
        var userName = UserName.Create("Maria da Silva");

        userName.FirstName.Should().Be("Maria");
        userName.Surname.Should().Be("da Silva");
        userName.GetInitials().Should().Be("MDS");
    }

    [Fact]
    public void Create_WithSingleName_ShouldThrow()
    {
        var action = () => UserName.Create("Maria");

        action.Should().Throw<UserNameInvalidException>();
    }
}

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldTrimValue()
    {
        var email = Email.Create("  maria@example.com  ");

        email.Value.Should().Be("maria@example.com");
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrow()
    {
        var action = () => Email.Create("maria.example.com");

        action.Should().Throw<InvalidEmailFormatException>();
    }
}

public class PhoneNumberTests
{
    [Fact]
    public void Create_WithFormattedValue_ShouldNormalizePhoneNumber()
    {
        var phoneNumber = PhoneNumber.Create("+55 16 91234-5678");

        phoneNumber.FormattedValue.Should().Be("+55 (16) 91234-5678");
        phoneNumber.CountryCode.Should().Be(55);
        phoneNumber.AreaCode.Should().Be(16);
    }

    [Fact]
    public void Create_WithCountryAreaAndValue_ShouldFormatPhoneNumber()
    {
        var phoneNumber = PhoneNumber.Create(55, 16, 912345678);

        phoneNumber.FormattedValue.Should().Be("+55 (16) 91234-5678");
        phoneNumber.RawValue.Should().Be(912345678);
    }

    [Fact]
    public void Create_WithInvalidPhoneNumber_ShouldThrow()
    {
        var action = () => PhoneNumber.Create("99999999");

        action.Should().Throw<InvalidPhoneNumberFormatException>();
    }
}
