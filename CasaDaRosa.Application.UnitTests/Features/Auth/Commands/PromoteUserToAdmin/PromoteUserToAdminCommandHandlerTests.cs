using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Commands.PromoteUserToAdmin;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Auth.Commands.PromoteUserToAdmin;

public class PromoteUserToAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCurrentUserIsAdminAndTargetExists_ShouldPromoteUser()
    {
        var currentUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var currentUser = User.CreateAdmin("Admin Atual", "admin@example.com", "hashed-password");
        var targetUser = User.Create("Maria Silva", "maria@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUserId };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetUser);

        var handler = new PromoteUserToAdminCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        await handler.Handle(new PromoteUserToAdminCommand(targetUserId), CancellationToken.None);

        targetUser.Role.Should().Be(UserRole.Admin);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNotAdmin_ShouldThrowForbidden()
    {
        var currentUserId = Guid.NewGuid();
        var currentUser = User.Create("Cliente Silva", "cliente@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUserId };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        var handler = new PromoteUserToAdminCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new PromoteUserToAdminCommand(Guid.NewGuid()), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenTargetUserDoesNotExist_ShouldThrowNotFound()
    {
        var currentUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var currentUser = User.CreateAdmin("Admin Atual", "admin@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUserId };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new PromoteUserToAdminCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new PromoteUserToAdminCommand(targetUserId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}
