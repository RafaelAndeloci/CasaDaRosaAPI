using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.ActivateCategory;

public sealed class ActivateCategoryCommandValidator : AbstractValidator<ActivateCategoryCommand>
{
    public ActivateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
