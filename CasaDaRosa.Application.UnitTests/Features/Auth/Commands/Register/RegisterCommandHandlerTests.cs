using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Commands.Register;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmailIsAvailable_ShouldPersistUserAndSendConfirmation()
    {
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var authEmailService = new Mock<IAuthEmailService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        securityService
            .Setup(service => service.HashPassword("plain-password"))
            .Returns("hashed-password");

        var handler = new RegisterCommandHandler(userRepository.Object, securityService.Object, authEmailService.Object, unitOfWork.Object);

        var response = await handler.Handle(new RegisterCommand("Maria da Silva", "maria@example.com", "plain-password", null), CancellationToken.None);

        response.UserId.Should().NotBeEmpty();
        userRepository.Verify(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        authEmailService.Verify(service => service.SendEmailConfirmationAsync(It.IsAny<CasaDaRosa.Application.Abstractions.Auth.SendEmailConfirmationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowConflict()
    {
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var authEmailService = new Mock<IAuthEmailService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(User.Create("Maria da Silva", "maria@example.com", "hashed-password"));

        var handler = new RegisterCommandHandler(userRepository.Object, securityService.Object, authEmailService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new RegisterCommand("Maria da Silva", "maria@example.com", "plain-password", null), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictApplicationException>();
    }
}
