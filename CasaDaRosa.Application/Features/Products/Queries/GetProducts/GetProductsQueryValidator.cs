using FluentValidation;

namespace CasaDaRosa.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Name)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}
