using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Commands.CreateAdmin;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Auth.Commands.CreateAdmin;

public class CreateAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCurrentUserIsAdminAndEmailIsAvailable_ShouldCreateAdmin()
    {
        var currentUserId = Guid.NewGuid();
        var currentUser = User.CreateAdmin("Admin Atual", "admin@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUserId };
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetByEmailAsync("novo-admin@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        securityService
            .Setup(service => service.HashPassword("plain-password"))
            .Returns("hashed-password");

        var handler = new CreateAdminCommandHandler(userContext, userRepository.Object, securityService.Object, unitOfWork.Object);

        var response = await handler.Handle(new CreateAdminCommand("Novo Admin", "novo-admin@example.com", "plain-password", "+55 (16) 99999-9999"), CancellationToken.None);

        response.UserId.Should().NotBeEmpty();
        userRepository.Verify(repository => repository.AddAsync(
            It.Is<User>(user =>
                user.Role == UserRole.Admin
                && user.Status == UserStatus.Active
                && user.Email.ToString() == "novo-admin@example.com"),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNotAdmin_ShouldThrowForbidden()
    {
        var currentUserId = Guid.NewGuid();
        var currentUser = User.Create("Cliente Silva", "cliente@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUserId };
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        var handler = new CreateAdminCommandHandler(userContext, userRepository.Object, securityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new CreateAdminCommand("Novo Admin", "novo-admin@example.com", "plain-password", null), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowConflict()
    {
        var currentUserId = Guid.NewGuid();
        var currentUser = User.CreateAdmin("Admin Atual", "admin@example.com", "hashed-password");
        var existingUser = User.Create("Maria Silva", "novo-admin@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUserId };
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetByEmailAsync("novo-admin@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var handler = new CreateAdminCommandHandler(userContext, userRepository.Object, securityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new CreateAdminCommand("Novo Admin", "novo-admin@example.com", "plain-password", null), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictApplicationException>();
    }
}
