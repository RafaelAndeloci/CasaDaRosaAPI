using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Auth;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Auth.Commands.ConfirmEmail;
using CasaDaRosa.Application.Features.Auth.Commands.CreateAdmin;
using CasaDaRosa.Application.Features.Auth.Commands.PromoteUserToAdmin;
using CasaDaRosa.Application.Features.Auth.Commands.ResendConfirmation;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Auth.Commands;

public class ConfirmEmailCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_ShouldConfirmEmailAndSaveChanges()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByEmailAsync(user.Email.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new ConfirmEmailCommandHandler(userRepository.Object, unitOfWork.Object);

        await handler.Handle(new ConfirmEmailCommand(user.Email.ToString(), user.EmailConfirmationToken), CancellationToken.None);

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
            .Setup(repository => repository.GetTrackedByEmailAsync("missing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ConfirmEmailCommandHandler(userRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ConfirmEmailCommand("missing@example.com", "token"), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}

public class ResendConfirmationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_ShouldRenewConfirmationAndSendEmail()
    {
        var user = User.Create("Maria da Silva", "maria@example.com", "hashed-password");
        var previousToken = user.EmailConfirmationToken;
        var userRepository = new Mock<IUserRepository>();
        var authEmailService = new Mock<IAuthEmailService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByEmailAsync(user.Email.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new ResendConfirmationCommandHandler(userRepository.Object, authEmailService.Object, unitOfWork.Object);

        await handler.Handle(new ResendConfirmationCommand(user.Email.ToString()), CancellationToken.None);

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
            .Setup(repository => repository.GetTrackedByEmailAsync("missing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ResendConfirmationCommandHandler(userRepository.Object, authEmailService.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ResendConfirmationCommand("missing@example.com"), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}

public class CreateAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCurrentUserIsAdmin_ShouldCreateAdminAndSaveChanges()
    {
        var currentUser = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUser.Id };
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetByEmailAsync("new-admin@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        securityService
            .Setup(service => service.HashPassword("plain-password"))
            .Returns("hashed-password");

        var handler = new CreateAdminCommandHandler(userContext, userRepository.Object, securityService.Object, unitOfWork.Object);

        var response = await handler.Handle(
            new CreateAdminCommand("New Admin", "new-admin@example.com", "plain-password", "+55 (16) 99999-9999"),
            CancellationToken.None);

        response.UserId.Should().NotBeEmpty();
        userRepository.Verify(repository => repository.AddAsync(
            It.Is<User>(user =>
                user.Id == response.UserId
                && user.Role == UserRole.Admin
                && user.Status == UserStatus.Active
                && user.Email.ToString() == "new-admin@example.com"
                && user.PasswordHash == "hashed-password"),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNotAdmin_ShouldThrowForbidden()
    {
        var currentUser = CreateActiveCustomer("customer@example.com");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUser.Id };
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        var handler = new CreateAdminCommandHandler(userContext, userRepository.Object, securityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(
            new CreateAdminCommand("New Admin", "new-admin@example.com", "plain-password", null),
            CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowConflict()
    {
        var currentUser = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");
        var existingUser = CreateActiveCustomer("existing@example.com");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUser.Id };
        var userRepository = new Mock<IUserRepository>();
        var securityService = new Mock<ISecurityService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetByEmailAsync(existingUser.Email.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var handler = new CreateAdminCommandHandler(userContext, userRepository.Object, securityService.Object, unitOfWork.Object);

        var action = () => handler.Handle(
            new CreateAdminCommand("New Admin", existingUser.Email.ToString(), "plain-password", null),
            CancellationToken.None);

        await action.Should().ThrowAsync<ConflictApplicationException>();
    }

    private static User CreateActiveCustomer(string email)
    {
        var user = User.Create("Maria da Silva", email, "hashed-password");
        user.ConfirmEmail(user.EmailConfirmationToken);
        return user;
    }
}

public class PromoteUserToAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCurrentUserIsAdmin_ShouldPromoteTargetUser()
    {
        var currentUser = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");
        var targetUser = CreateActiveCustomer("customer@example.com");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUser.Id };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(targetUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetUser);

        var handler = new PromoteUserToAdminCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        await handler.Handle(new PromoteUserToAdminCommand(targetUser.Id), CancellationToken.None);

        targetUser.Role.Should().Be(UserRole.Admin);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNotAdmin_ShouldThrowForbidden()
    {
        var currentUser = CreateActiveCustomer("customer@example.com");
        var targetUser = CreateActiveCustomer("another@example.com");
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUser.Id };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(targetUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetUser);

        var handler = new PromoteUserToAdminCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new PromoteUserToAdminCommand(targetUser.Id), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenTargetUserDoesNotExist_ShouldThrowNotFound()
    {
        var currentUser = User.CreateAdmin("Admin Master", "admin@example.com", "hashed-password");
        var targetUserId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, UserId = currentUser.Id };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(currentUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);
        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new PromoteUserToAdminCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new PromoteUserToAdminCommand(targetUserId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    private static User CreateActiveCustomer(string email)
    {
        var user = User.Create("Maria da Silva", email, "hashed-password");
        user.ConfirmEmail(user.EmailConfirmationToken);
        return user;
    }
}
