using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
