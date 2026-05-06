using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities;

public class Review : AuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal RatingValue { get; private set; }
    public string? Comment { get; private set; }

    private Review()
    {
    }

    public Review(Guid productId, Guid userId, Rating rating, string? comment)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainValidationException("review.product.invalid", "Review product is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("review.user.invalid", "Review user is required.");
        }

        ProductId = productId;
        UserId = userId;
        RatingValue = rating.Value;
        Comment = ValidateComment(comment);
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
