using CasaDaRosa.Domain.Entities.Addresses;
using CasaDaRosa.Domain.Entities.Addresses.Exceptions;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.Entities.Addresses;

public class AddressTests
{
    [Fact]
    public void Create_ShouldInitializeAddressWithProvidedValues()
    {
        var userId = Guid.NewGuid();

        var address = Address.Create(
            userId,
            "Rua das Flores",
            123,
            "Centro",
            "Ribeirão Preto",
            "SP",
            "14000-000",
            "Apto 12",
            "Próximo à praça",
            true);

        address.UserId.Should().Be(userId);
        address.Street.ToString().Should().Be("Rua das Flores");
        address.Number.ToString().Should().Be("123");
        address.Neighborhood.Should().Be("Centro");
        address.City.Should().Be("Ribeirão Preto");
        address.State.Abbreviation.Code.Should().Be("SP");
        address.ZipCode.ToString().Should().Be("14000-000");
        address.Complement.Should().Be("Apto 12");
        address.Reference.Should().Be("Próximo à praça");
        address.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void Create_WhenStreetIsEmpty_ShouldThrow()
    {
        var action = () => Address.Create(
            Guid.NewGuid(),
            string.Empty,
            123,
            "Centro",
            "Ribeirão Preto",
            "SP",
            "14000-000",
            null,
            null,
            false);

        action.Should().Throw<StreetRequiredException>();
    }

    [Fact]
    public void Create_WhenNumberIsInvalid_ShouldThrow()
    {
        var action = () => Address.Create(
            Guid.NewGuid(),
            "Rua das Flores",
            0,
            "Centro",
            "Ribeirão Preto",
            "SP",
            "14000-000",
            null,
            null,
            false);

        action.Should().Throw<AddressNumberMustBeGreaterThanZeroException>();
    }

    [Fact]
    public void Create_WhenZipCodeFormatIsInvalid_ShouldThrow()
    {
        var action = () => Address.Create(
            Guid.NewGuid(),
            "Rua das Flores",
            123,
            "Centro",
            "Ribeirão Preto",
            "SP",
            "14000000",
            null,
            null,
            false);

        action.Should().Throw<ZipCodeInvalidFormatException>();
    }
}
