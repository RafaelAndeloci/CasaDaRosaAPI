using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products.Exceptions;

public sealed class ProductCategoryRequiredException()
    : DomainValidationException("product.category.invalid", "Product category is required.");

public sealed class ProductReviewRequiredException()
    : DomainValidationException("product.review.required", "Review is required.");

public sealed class ProductNameRequiredException()
    : DomainValidationException("product.name.invalid", "Product name is required.");

public sealed class ProductNameTooLongException()
    : DomainValidationException("product.name.invalid", "Product name must have a maximum of 150 characters.");

public sealed class ProductDescriptionEmptyException()
    : DomainValidationException("product.description.invalid", "Product description cannot be empty.");

public sealed class ProductDescriptionTooLongException()
    : DomainValidationException("product.description.invalid", "Product description must have a maximum of 1000 characters.");

public sealed class ProductRatingOutOfRangeException()
    : DomainValidationException("product.rating.invalid", "Rating must be between 0 and 5 in increments of 0.5.");

public sealed class ReviewCommentTooLongException()
    : DomainValidationException("review.comment.invalid", "Review comment must have a maximum of 1000 characters.");
