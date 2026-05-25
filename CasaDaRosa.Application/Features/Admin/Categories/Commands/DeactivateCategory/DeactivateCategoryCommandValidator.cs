using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.DeactivateCategory;

public sealed class DeactivateCategoryCommandValidator : AbstractValidator<DeactivateCategoryCommand>
{
    public DeactivateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
