using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Features.Categories.Queries.GetCategories;
using CasaDaRosa.Application.Features.Products.Queries.GetProductById;
using CasaDaRosa.Application.Features.Products.Queries.GetProducts;
using CasaDaRosa.Domain.Entities.Categories;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Catalog;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFilterAndPaginateCategories()
    {
        var categoryRepository = new Mock<ICategoryRepository>();
        var categories = new[]
        {
            Category.Create("Rosas", "Categoria 1", true),
            Category.Create("Tulipas", "Categoria 2", true),
            Category.Create("Rosas do Campo", "Categoria 3", false)
        };

        categoryRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var handler = new GetCategoriesQueryHandler(categoryRepository.Object);

        var response = await handler.Handle(new GetCategoriesQuery(Name: "rosa", PageNumber: 1, PageSize: 1), CancellationToken.None);

        response.TotalCount.Should().Be(2);
        response.Items.Should().ContainSingle();
        response.Items.Single().Name.Should().Contain("Rosas");
    }
}

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFilterByCategoryNameAndPaginate()
    {
        var categoryId = Guid.NewGuid();
        var anotherCategoryId = Guid.NewGuid();
        var productRepository = new Mock<IProductRepository>();
        var products = new[]
        {
            Product.Create(categoryId, "Buquê de Rosas", "Desc", new Money(50m, Currency.Brl), 10),
            Product.Create(categoryId, "Arranjo de Rosas", "Desc", new Money(70m, Currency.Brl), 8),
            Product.Create(anotherCategoryId, "Tulipa Azul", "Desc", new Money(30m, Currency.Brl), 5)
        };

        productRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetProductsQueryHandler(productRepository.Object);

        var response = await handler.Handle(new GetProductsQuery(Name: "rosa", CategoryId: categoryId, PageNumber: 1, PageSize: 1), CancellationToken.None);

        response.TotalCount.Should().Be(2);
        response.Items.Should().ContainSingle();
        response.Items.Single().CategoryId.Should().Be(categoryId);
        response.Items.Single().Name.Should().Be("Arranjo de Rosas");
    }
}

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnNull()
    {
        var productRepository = new Mock<IProductRepository>();
        var productId = Guid.NewGuid();

        productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(productRepository.Object);

        var response = await handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        response.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnMappedResponse()
    {
        var categoryId = Guid.NewGuid();
        var product = Product.Create(categoryId, "Buquê Premium", "Desc", new Money(99.9m, Currency.Brl), 3);
        var productRepository = new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new GetProductByIdQueryHandler(productRepository.Object);

        var response = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        response.Should().NotBeNull();
        response!.Id.Should().Be(product.Id);
        response.Name.Should().Be(product.Name.ToString());
        response.Price.Should().Be(product.Price.Amount);
        response.StockQuantity.Should().Be(product.StockQuantity.Value);
    }
}
