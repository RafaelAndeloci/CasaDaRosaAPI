using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Users.Commands.ActivateUser;
using CasaDaRosa.Application.Features.Admin.Users.Commands.DeactivateUser;
using CasaDaRosa.Application.Features.Admin.Users.Queries.GetUserById;
using CasaDaRosa.Application.Features.Admin.Users.Queries.GetUsers;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Users;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Admin.Users;

public class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ShouldThrowForbidden()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = false };
        var userRepository = new Mock<IUserRepository>();
        var handler = new GetUsersQueryHandler(userContext, userRepository.Object);

        var action = () => handler.Handle(new GetUsersQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_ShouldFilterAndPaginateUsers()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var userRepository = new Mock<IUserRepository>();
        var admin = User.CreateAdmin("Ana Admin", "ana@example.com", "hashed-password");
        var customer = User.Create("Bruno Cliente", "bruno@example.com", "hashed-password");

        userRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { customer, admin });

        var handler = new GetUsersQueryHandler(userContext, userRepository.Object);

        var response = await handler.Handle(new GetUsersQuery(Search: "ana", RoleId: (int)UserRole.Admin, PageNumber: 1, PageSize: 10), CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().Email.Should().Be("ana@example.com");
    }
}

public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var userRepository = new Mock<IUserRepository>();
        var userId = Guid.NewGuid();

        userRepository
            .Setup(repository => repository.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new GetUserByIdQueryHandler(userContext, userRepository.Object);

        var action = () => handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnMappedResponse()
    {
        var user = User.CreateAdmin("Ana Admin", "ana@example.com", "hashed-password", "+55 (16) 99999-9999");
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var userRepository = new Mock<IUserRepository>();

        userRepository
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetUserByIdQueryHandler(userContext, userRepository.Object);

        var response = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        response.Id.Should().Be(user.Id);
        response.Email.Should().Be(user.Email.ToString());
        response.Role.Id.Should().Be((int)UserRole.Admin);
    }
}

public class ActivateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_ShouldActivateAndSaveChanges()
    {
        var user = User.Create("Maria Silva", "maria@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new ActivateUserCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        await handler.Handle(new ActivateUserCommand(user.Id), CancellationToken.None);

        user.Status.Should().Be(UserStatus.Active);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class DeactivateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var userId = Guid.NewGuid();

        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new DeactivateUserCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new DeactivateUserCommand(userId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldDeactivateAndSaveChanges()
    {
        var user = User.CreateAdmin("Ana Admin", "ana@example.com", "hashed-password");
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(repository => repository.GetTrackedByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new DeactivateUserCommandHandler(userContext, userRepository.Object, unitOfWork.Object);

        await handler.Handle(new DeactivateUserCommand(user.Id), CancellationToken.None);

        user.Status.Should().Be(UserStatus.Inactive);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
