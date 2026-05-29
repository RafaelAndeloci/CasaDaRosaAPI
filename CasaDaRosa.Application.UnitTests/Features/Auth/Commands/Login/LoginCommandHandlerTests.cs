using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Auth;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Commands.Login;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnLoginResponse()
    {
        var user = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        userRepository
            .Setup(repository => repository.GetByEmailAsync("admin@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        securityService
            .Setup(service => service.VerifyPassword("plain-password", "hashed-password"))
            .Returns(true);

        jwtTokenGenerator
            .Setup(generator => generator.GenerateToken(user.Id, user.Email.ToString(), It.IsAny<IEnumerable<string>>()))
            .Returns(new AuthTokenResult("jwt-token", DateTime.UtcNow.AddHours(1)));

        var handler = new LoginCommandHandler(userRepository.Object, securityService.Object, jwtTokenGenerator.Object);

        var response = await handler.Handle(new LoginCommand("admin@example.com", "plain-password"), CancellationToken.None);

        response.Auth.AccessToken.Should().Be("jwt-token");
        response.Auth.User.Email.Should().Be("admin@example.com");
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ShouldThrowUnauthorized()
    {
        var user = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        userRepository
            .Setup(repository => repository.GetByEmailAsync("admin@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        securityService
            .Setup(service => service.VerifyPassword("wrong-password", "hashed-password"))
            .Returns(false);

        var handler = new LoginCommandHandler(userRepository.Object, securityService.Object, jwtTokenGenerator.Object);

        var action = () => handler.Handle(new LoginCommand("admin@example.com", "wrong-password"), CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedApplicationException>();
    }

    [Fact]
    public async Task Handle_WithPendingConfirmationUser_ShouldThrowForbidden()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        userRepository
            .Setup(repository => repository.GetByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        securityService
            .Setup(service => service.VerifyPassword("plain-password", "hashed-password"))
            .Returns(true);

        var handler = new LoginCommandHandler(userRepository.Object, securityService.Object, jwtTokenGenerator.Object);

        var action = () => handler.Handle(new LoginCommand("maria@example.com", "plain-password"), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }
}
