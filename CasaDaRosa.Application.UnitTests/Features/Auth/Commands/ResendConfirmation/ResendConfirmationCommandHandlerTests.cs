using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Auth;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Commands.ResendConfirmation;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Auth.Commands.ResendConfirmation;

public class ResendConfirmationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_ShouldRenewTokenSaveAndSendEmail()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var previousToken = user.EmailConfirmationToken;
        var userRepository = new Mock<IUserRepository>();
        var authEmailService = new Mock<IAuthEmailService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new ResendConfirmationCommandHandler(userRepository.Object, authEmailService.Object, unitOfWork.Object);

        await handler.Handle(new ResendConfirmationCommand("maria@example.com"), CancellationToken.None);

        user.EmailConfirmationToken.Should().NotBe(previousToken);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        authEmailService.Verify(service => service.SendEmailConfirmationAsync(
            It.Is<SendEmailConfirmationRequest>(request =>
                request.UserId == user.Id
                && request.FullName == user.Name.ToString()
                && request.Email == user.Email.ToString()
                && request.ConfirmationToken == user.EmailConfirmationToken),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFound()
    {
        var userRepository = new Mock<IUserRepository>();
        var authEmailService = new Mock<IAuthEmailService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ResendConfirmationCommandHandler(userRepository.Object, authEmailService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ResendConfirmationCommand("maria@example.com"), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}
