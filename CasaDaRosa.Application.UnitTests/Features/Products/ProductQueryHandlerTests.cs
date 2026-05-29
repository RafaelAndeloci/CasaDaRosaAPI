using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Features.Products.Queries.GetProductById;
using CasaDaRosa.Application.Features.Products.Queries.GetProducts;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Products;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnNull()
    {
        var productRepository = new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(productRepository.Object);

        var response = await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        response.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnDetails()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", "Desc", new Money(45m, Currency.Brl), 7);
        var productRepository = new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new GetProductByIdQueryHandler(productRepository.Object);

        var response = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        response.Should().NotBeNull();
        response!.Id.Should().Be(product.Id);
        response.Price.Should().Be(45m);
        response.StockQuantity.Should().Be(7);
    }
}

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFilterAndPaginateProducts()
    {
        var categoryId = Guid.NewGuid();
        var matchingProduct = Product.Create(categoryId, "Buquê Especial", "Desc", new Money(45m, Currency.Brl), 7);
        var otherProduct = Product.Create(Guid.NewGuid(), "Tulipa", "Desc", new Money(20m, Currency.Brl), 5);
        var productRepository = new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { otherProduct, matchingProduct });

        var handler = new GetProductsQueryHandler(productRepository.Object);

        var response = await handler.Handle(new GetProductsQuery(Name: "buque", CategoryId: categoryId, PageNumber: 1, PageSize: 10), CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().Id.Should().Be(matchingProduct.Id);
    }
}
