using FluentValidation;

namespace CasaDaRosa.Application.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Name)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}
