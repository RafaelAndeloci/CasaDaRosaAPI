using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Commands.ConfirmEmail;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_ShouldConfirmEmailAndSaveChanges()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new ConfirmEmailCommandHandler(userRepository.Object, unitOfWork.Object);

        await handler.Handle(new ConfirmEmailCommand("maria@example.com", user.EmailConfirmationToken), CancellationToken.None);

        user.Status.Should().Be(UserStatus.Active);
        user.EmailConfirmationToken.Should().BeEmpty();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFound()
    {
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ConfirmEmailCommandHandler(userRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ConfirmEmailCommand("maria@example.com", "token"), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}
