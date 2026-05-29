using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Products.Commands.ActivateProduct;
using CasaDaRosa.Application.Features.Admin.Products.Commands.DeactivateProduct;
using CasaDaRosa.Application.Features.Admin.Products.Commands.UpdateProduct;
using CasaDaRosa.Application.Features.Admin.Products.Queries.GetProductById;
using CasaDaRosa.Application.Features.Admin.Products.Queries.GetProducts;
using CasaDaRosa.Application.UnitTests.TestDoubles;
using CasaDaRosa.Domain.Entities.Categories;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Admin.Products;

public class GetAdminProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ShouldThrowForbidden()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = false };
        var productRepository = new Mock<IProductRepository>();
        var handler = new GetAdminProductsQueryHandler(userContext, productRepository.Object);

        var action = () => handler.Handle(new GetAdminProductsQuery(), CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenApplicationException>();
    }

    [Fact]
    public async Task Handle_ShouldFilterAndPaginateProducts()
    {
        var categoryId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var productRepository = new Mock<IProductRepository>();
        var matchingProduct = Product.Create(categoryId, "Buquê Especial", "Desc", new Money(50m, Currency.Brl), 5);
        var otherProduct = Product.Create(Guid.NewGuid(), "Tulipa", "Desc", new Money(20m, Currency.Brl), 3);

        productRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { otherProduct, matchingProduct });

        var handler = new GetAdminProductsQueryHandler(userContext, productRepository.Object);

        var response = await handler.Handle(new GetAdminProductsQuery(Name: "buque", CategoryId: categoryId, IsActive: true, PageNumber: 1, PageSize: 10), CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().Id.Should().Be(matchingProduct.Id);
    }
}

public class GetAdminProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFound()
    {
        var productId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var productRepository = new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new GetAdminProductByIdQueryHandler(userContext, productRepository.Object);

        var action = () => handler.Handle(new GetAdminProductByIdQuery(productId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnMappedResponse()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", "Desc", new Money(45m, Currency.Brl), 7);
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var productRepository = new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new GetAdminProductByIdQueryHandler(userContext, productRepository.Object);

        var response = await handler.Handle(new GetAdminProductByIdQuery(product.Id), CancellationToken.None);

        response.Id.Should().Be(product.Id);
        response.Name.Should().Be("Buquê Especial");
        response.Price.Should().Be(45m);
        response.StockQuantity.Should().Be(7);
    }
}

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductAndCategoryExist_ShouldUpdateAndReturnResponse()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Rosas", null, true);
        var product = Product.Create(Guid.NewGuid(), "Buquê", "Desc", new Money(20m, Currency.Brl), 2);
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var categoryRepository = new Mock<ICategoryRepository>();
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        productRepository
            .Setup(repository => repository.GetTrackedByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductCommandHandler(userContext, categoryRepository.Object, productRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(new UpdateProductCommand(product.Id, categoryId, "Buquê Premium", "Nova desc", 35m, 8, false), CancellationToken.None);

        response.Name.Should().Be("Buquê Premium");
        response.StockQuantity.Should().Be(8);
        response.IsActive.Should().BeFalse();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

        var handler = new UpdateProductCommandHandler(userContext, categoryRepository.Object, productRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new UpdateProductCommand(Guid.NewGuid(), Guid.NewGuid(), "Buquê", null, 20m, 1, true), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}

public class ActivateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductExists_ShouldActivateAndSaveChanges()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê", null, new Money(20m, Currency.Brl), 1);
        product.Deactivate();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetTrackedByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new ActivateProductCommandHandler(userContext, productRepository.Object, unitOfWork.Object);

        await handler.Handle(new ActivateProductCommand(product.Id), CancellationToken.None);

        product.IsActive.Should().BeTrue();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFound()
    {
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var productId = Guid.NewGuid();

        productRepository
            .Setup(repository => repository.GetTrackedByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new ActivateProductCommandHandler(userContext, productRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new ActivateProductCommand(productId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }
}

public class DeactivateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFound()
    {
        var productId = Guid.NewGuid();
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetTrackedByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new DeactivateProductCommandHandler(userContext, productRepository.Object, unitOfWork.Object);

        var action = () => handler.Handle(new DeactivateProductCommand(productId), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundApplicationException>();
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldDeactivateAndSaveChanges()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê", null, new Money(20m, Currency.Brl), 1);
        var userContext = new FakeUserContext { IsAuthenticated = true, IsAdmin = true };
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetTrackedByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new DeactivateProductCommandHandler(userContext, productRepository.Object, unitOfWork.Object);

        await handler.Handle(new DeactivateProductCommand(product.Id), CancellationToken.None);

        product.IsActive.Should().BeFalse();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
