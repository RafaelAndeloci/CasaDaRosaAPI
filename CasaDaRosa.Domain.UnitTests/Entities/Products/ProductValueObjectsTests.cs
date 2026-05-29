using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.Entities.Products.Exceptions;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.Entities.Products;

public class RatingTests
{
    [Fact]
    public void Create_WithHalfStepValue_ShouldCreateRating()
    {
        var rating = Rating.Create(4.5m);

        rating.Value.Should().Be(4.5m);
    }

    [Fact]
    public void Create_WithInvalidStep_ShouldThrow()
    {
        var action = () => Rating.Create(4.3m);

        action.Should().Throw<ProductRatingOutOfRangeException>();
    }

    [Fact]
    public void Create_WithValueOutOfRange_ShouldThrow()
    {
        var action = () => Rating.Create(5.5m);

        action.Should().Throw<ProductRatingOutOfRangeException>();
    }
}

public class ReviewTests
{
    [Fact]
    public void Create_WithBlankComment_ShouldStoreNullComment()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 4m, "   ");

        review.Comment.Should().BeNull();
    }

    [Fact]
    public void Create_WithCommentTooLong_ShouldThrow()
    {
        var action = () => Review.Create(Guid.NewGuid(), Guid.NewGuid(), 4m, new string('a', 1001));

        action.Should().Throw<ReviewCommentTooLongException>();
    }
}
