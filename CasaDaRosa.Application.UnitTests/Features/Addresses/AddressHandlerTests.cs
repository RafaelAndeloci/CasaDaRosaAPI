using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Addresses.Commands.CreateAddress;
using CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Addresses;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Addresses;

public class CreateAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsAuthenticated_ShouldCreateAddressAndSaveChanges()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var addressRepository = new Mock<IAddressRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new CreateAddressCommandHandler(userContext, addressRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(
            new CreateAddressCommand(
                "Rua das Flores",
                123,
                "Centro",
                "Ribeirão Preto",
                "SP",
                "14000-000",
                "Apto 12",
                "Próximo à praça",
                true),
            CancellationToken.None);

        response.Id.Should().NotBeEmpty();
        addressRepository.Verify(repository => repository.AddAsync(
            It.Is<Address>(address =>
                address.Id == response.Id
                && address.UserId == userId
                && address.Street.ToString() == "Rua das Flores"
                && address.Number.ToString() == "123"
                && address.City == "Ribeirão Preto"
                && address.State.Abbreviation.Code == "SP"
                && address.ZipCode.ToString() == "14000-000"
                && address.IsDefault),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorized()
    {
        var userContext = new FakeUserContext { IsAuthenticated = false };
        var addressRepository = new Mock<IAddressRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateAddressCommandHandler(userContext, addressRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(
            new CreateAddressCommand("Rua das Flores", 123, "Centro", "Ribeirão Preto", "SP", "14000-000", null, null, false),
            CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedApplicationException>();
    }
}

public class GetMyAddressesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorized()
    {
        var userContext = new FakeUserContext { IsAuthenticated = false };
        var addressRepository = new Mock<IAddressRepository>();
        var handler = new GetMyAddressesQueryHandler(userContext, addressRepository.Object);

        var action = () => handler.Handle(new GetMyAddressesQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedApplicationException>();
    }

    [Fact]
    public async Task Handle_ShouldFilterAndPaginateAddresses()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var addressRepository = new Mock<IAddressRepository>();
        var addresses = new[]
        {
            Address.Create(userId, "Rua A", 10, "Centro", "Ribeirão Preto", "SP", "14000-000", null, null, true),
            Address.Create(userId, "Rua B", 20, "Jardim", "Campinas", "SP", "13000-000", null, null, false),
            Address.Create(userId, "Rua C", 30, "Centro", "Ribeirão Preto", "RJ", "20000-000", null, null, false)
        };

        addressRepository
            .Setup(repository => repository.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addresses);

        var handler = new GetMyAddressesQueryHandler(userContext, addressRepository.Object);

        var response = await handler.Handle(new GetMyAddressesQuery(City: "ribeirao", State: "sp", PageNumber: 1, PageSize: 1), CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.PageNumber.Should().Be(1);
        response.PageSize.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().City.Should().Be("Ribeirão Preto");
        response.Items.Single().State.Should().Be("SP");
    }
}
