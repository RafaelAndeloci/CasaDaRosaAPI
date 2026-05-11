using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

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
        var validatedComment = ValidateComment(comment);
        return new Review(
            id: Guid.NewGuid(),
            productId: productId,
            userId: userId, 
            ratingValue: ratingValue, 
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
            throw new DomainValidationException("review.comment.invalid", "Review comment must have a maximum of 1000 characters.");
        }

        return normalizedComment;
    }
}
