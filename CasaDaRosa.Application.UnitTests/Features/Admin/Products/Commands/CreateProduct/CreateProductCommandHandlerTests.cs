using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Products.Commands.CreateProduct;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Categories;
using CasaDaRosa.Domain.Entities.Products;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Admin.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsAdminAndCategoryExists_ShouldPersistProduct()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var categoryId = Guid.NewGuid();

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Category.Create("Rosas", null, true));

        var handler = new CreateProductCommandHandler(userContext, categoryRepository.Object, productRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(new CreateProductCommand(categoryId, "Buquê Especial", "Com rosas", 29.90m, 8, true), CancellationToken.None);

        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be("Buquê Especial");
        productRepository.Verify(repository => repository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ShouldThrowForbidden()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = false };
        var categoryRepository = new Mock<ICategoryRepository>();
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new CreateProductCommandHandler(userContext, categoryRepository.Object, productRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new CreateProductCommand(Guid.NewGuid(), "Buquê Especial", null, 29.90m, 8, true), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new CreateProductCommandHandler(userContext, categoryRepository.Object, productRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new CreateProductCommand(Guid.NewGuid(), "Buquê Especial", null, 29.90m, 8, true), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}
