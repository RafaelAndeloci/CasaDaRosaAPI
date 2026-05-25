using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategories;

public sealed class GetAdminCategoriesQueryValidator : AbstractValidator<GetAdminCategoriesQuery>
{
    public GetAdminCategoriesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Name)
            .MaximumLength(120)
            .When(x => x.Name is not null);
    }
}
