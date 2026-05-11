using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Categories.Exceptions;

public sealed class CategoryNameRequiredException()
    : DomainValidationException("category.name.invalid", "Category name is required.");

public sealed class CategoryNameTooLongException()
    : DomainValidationException("category.name.invalid", "Category name must have a maximum of 120 characters.");

public sealed class CategoryDescriptionEmptyException()
    : DomainValidationException("category.description.invalid", "Category description cannot be empty.");

public sealed class CategoryDescriptionTooLongException()
    : DomainValidationException("category.description.invalid", "Category description must have a maximum of 500 characters.");
