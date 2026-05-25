using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Products.Queries.GetProductById;

public sealed class GetAdminProductByIdQueryValidator : AbstractValidator<GetAdminProductByIdQuery>
{
    public GetAdminProductByIdQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}
