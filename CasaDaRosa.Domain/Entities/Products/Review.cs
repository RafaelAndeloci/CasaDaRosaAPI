using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Products.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products;

public class Review : AuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal RatingValue { get; private set; }
    public string? Comment { get; private set; }

    private Review(
        Guid id, 
        Guid productId, 
        Guid userId, 
        decimal ratingValue, 
        string? comment) : base(id)
    {
        ProductId = productId;
        UserId = userId;
        RatingValue = ratingValue;
        Comment = comment;
    }

    public static Review Create(
        Guid productId, 
        Guid userId, 
        decimal ratingValue, 
        string? comment)
    {
        if (productId == Guid.Empty)
        {
            throw new ReviewProductRequiredException();
        }

        if (userId == Guid.Empty)
        {
            throw new ReviewUserRequiredException();
        }

        var rating = Rating.Create(ratingValue);
        var validatedComment = ValidateComment(comment);

        return new Review(
            id: Guid.NewGuid(),
            productId: productId,
            userId: userId, 
            ratingValue: rating.Value, 
            comment: validatedComment);
    }

    private static string? ValidateComment(string? comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return null;
        }

        var normalizedComment = comment.Trim();

        if (normalizedComment.Length > 1000)
        {
            throw new ReviewCommentTooLongException();
        }

        return normalizedComment;
    }
}
