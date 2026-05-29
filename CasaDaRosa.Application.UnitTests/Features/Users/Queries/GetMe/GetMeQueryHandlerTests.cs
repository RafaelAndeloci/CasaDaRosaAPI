using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Users.Queries.GetMe;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Users.Queries.GetMe;

public class GetMeQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorized()
    {
        var userContext = new FakeUserContext { IsAuthenticated = false };
        var userRepository = new Mock<IUserRepository>();
        var handler = new GetMeQueryHandler(userContext, userRepository.Object);

        var action = () => handler.Handle(new GetMeQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var userId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = userId };
        var userRepository = new Mock<IUserRepository>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new GetMeQueryHandler(userContext, userRepository.Object);

        var response = await handler.Handle(new GetMeQuery(), CancellationToken.None);

        response.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnMappedProfile()
    {
        var user = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password", "+55 (16) 99999-9999");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = user.Id };
        var userRepository = new Mock<IUserRepository>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetMeQueryHandler(userContext, userRepository.Object);

        var response = await handler.Handle(new GetMeQuery(), CancellationToken.None);

        response.Should().NotBeNull();
        response!.Id.Should().Be(user.Id);
        response.FullName.Should().Be(user.Name.ToString());
        response.Email.Should().Be(user.Email.ToString());
        response.PhoneNumber.Should().Be(user.PhoneNumber!.ToString());
        response.Role.Id.Should().Be((int)user.Role);
        response.Status.Id.Should().Be((int)user.Status);
    }
}
