using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.Entities.Products.Exceptions;
using CasaDaRosa.Domain.ValueObjects;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.Entities.Products;

public class ProductTests
{
    [Fact]
    public void Create_ShouldInitializeActiveProduct()
    {
        var categoryId = Guid.NewGuid();
        var price = new Money(25.90m, Currency.Brl);

        var product = Product.Create(categoryId, "Buquê Especial", "Com rosas e lírios", price, 10);

        product.CategoryId.Should().Be(categoryId);
        product.Name.ToString().Should().Be("Buquê Especial");
        product.Description!.ToString().Should().Be("Com rosas e lírios");
        product.Price.Should().Be(price);
        product.StockQuantity.Value.Should().Be(10);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrow()
    {
        var action = () => Product.Create(Guid.NewGuid(), "Buquê Especial", null, new Money(25.90m, Currency.Brl), -1);

        action.Should().Throw<ProductStockQuantityInvalidDomainException>();
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateCoreFields()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", null, new Money(25.90m, Currency.Brl), 10);
        var newCategoryId = Guid.NewGuid();
        var newPrice = new Money(39.90m, Currency.Brl);

        product.UpdateDetails(newCategoryId, "Cesta Premium", "Com chocolates", newPrice);

        product.CategoryId.Should().Be(newCategoryId);
        product.Name.ToString().Should().Be("Cesta Premium");
        product.Description!.ToString().Should().Be("Com chocolates");
        product.Price.Should().Be(newPrice);
        product.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void UpdateStockQuantity_WithNegativeValue_ShouldReturnFailure()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", null, new Money(25.90m, Currency.Brl), 10);

        var result = product.UpdateStockQuantity(-5);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidStockQuantity);
    }

    [Fact]
    public void AddReview_WhenDuplicated_ShouldReturnFailure()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", null, new Money(25.90m, Currency.Brl), 10);
        var review = Review.Create(product.Id, Guid.NewGuid(), 4.5m, "Excelente");

        product.AddReview(review);
        var result = product.AddReview(review);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DuplicatedReview);
    }

    [Fact]
    public void AddReview_ShouldAcceptTrimmedComment()
    {
        var product = Product.Create(Guid.NewGuid(), "Buquê Especial", null, new Money(25.90m, Currency.Brl), 10);
        var review = Review.Create(product.Id, Guid.NewGuid(), 4.5m, "  Excelente produto  ");

        var result = product.AddReview(review);

        result.IsSuccess.Should().BeTrue();
        product.Reviews.Should().ContainSingle();
        product.Reviews.Single().Comment.Should().Be("Excelente produto");
    }
}
