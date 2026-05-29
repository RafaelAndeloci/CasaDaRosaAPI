using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.ActivateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.CreateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.DeactivateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.UpdateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategories;
using CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategoryById;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Categories;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Admin.Categories;

public class GetAdminCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ShouldThrowForbidden()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = false };
        var categoryRepository = new Mock<ICategoryRepository>();
        var handler = new GetAdminCategoriesQueryHandler(userContext, categoryRepository.Object);

        var action = () => handler.Handle(new GetAdminCategoriesQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_ShouldFilterAndPaginateCategories()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var activeCategory = Category.Create("Rosas", "Categoria ativa", true);
        var inactiveCategory = Category.Create("Tulipas", "Categoria inativa", false);

        categoryRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { inactiveCategory, activeCategory });

        var handler = new GetAdminCategoriesQueryHandler(userContext, categoryRepository.Object);

        var response = await handler.Handle(new GetAdminCategoriesQuery(Name: "rosa", IsActive: true, PageNumber: 1, PageSize: 10), CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().Name.Should().Be("Rosas");
    }
}

public class GetCategoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowNotFound()
    {
        var categoryId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new GetCategoryByIdQueryHandler(userContext, categoryRepository.Object);

        var action = () => handler.Handle(new GetCategoryByIdQuery(categoryId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldReturnMappedResponse()
    {
        var category = Category.Create("Rosas", "Categoria ativa", true);
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new GetCategoryByIdQueryHandler(userContext, categoryRepository.Object);

        var response = await handler.Handle(new GetCategoryByIdQuery(category.Id), CancellationToken.None);

        response.Id.Should().Be(category.Id);
        response.Name.Should().Be("Rosas");
        response.Description.Should().Be("Categoria ativa");
        response.IsActive.Should().BeTrue();
    }
}

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsAdmin_ShouldPersistCategory()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateCategoryCommandHandler(userContext, categoryRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(new CreateCategoryCommand("Rosas", "Categoria ativa", true), CancellationToken.None);

        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be("Rosas");
        categoryRepository.Verify(repository => repository.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class UpdateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldUpdateDetailsAndStatus()
    {
        var category = Category.Create("Rosas", "Categoria antiga", false);
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        categoryRepository
            .Setup(repository => repository.GetTrackedByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new UpdateCategoryCommandHandler(userContext, categoryRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(new UpdateCategoryCommand(category.Id, "Rosas Premium", "Nova descrição", true), CancellationToken.None);

        response.Name.Should().Be("Rosas Premium");
        response.IsActive.Should().BeTrue();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var categoryId = Guid.NewGuid();

        categoryRepository
            .Setup(repository => repository.GetTrackedByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new UpdateCategoryCommandHandler(userContext, categoryRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new UpdateCategoryCommand(categoryId, "Rosas", null, true), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}

public class ActivateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldActivateAndSaveChanges()
    {
        var category = Category.Create("Rosas", null, false);
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        categoryRepository
            .Setup(repository => repository.GetTrackedByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new ActivateCategoryCommandHandler(userContext, categoryRepository.Object, unitOfWork.Object);

        await handler.Handle(new ActivateCategoryCommand(category.Id), CancellationToken.None);

        category.IsActive.Should().BeTrue();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var categoryId = Guid.NewGuid();

        categoryRepository
            .Setup(repository => repository.GetTrackedByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new ActivateCategoryCommandHandler(userContext, categoryRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ActivateCategoryCommand(categoryId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}

public class DeactivateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowNotFound()
    {
        var categoryId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        categoryRepository
            .Setup(repository => repository.GetTrackedByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new DeactivateCategoryCommandHandler(userContext, categoryRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new DeactivateCategoryCommand(categoryId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldDeactivateAndSaveChanges()
    {
        var category = Category.Create("Rosas", null, true);
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        categoryRepository
            .Setup(repository => repository.GetTrackedByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new DeactivateCategoryCommandHandler(userContext, categoryRepository.Object, unitOfWork.Object);

        await handler.Handle(new DeactivateCategoryCommand(category.Id), CancellationToken.None);

        category.IsActive.Should().BeFalse();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
