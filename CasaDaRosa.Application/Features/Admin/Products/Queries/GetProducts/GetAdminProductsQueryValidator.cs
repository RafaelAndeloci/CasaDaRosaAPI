using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Products.Queries.GetProducts;

public sealed class GetAdminProductsQueryValidator : AbstractValidator<GetAdminProductsQuery>
{
    public GetAdminProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Name)
            .MaximumLength(150)
            .When(x => x.Name is not null);
    }
}
