using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.DeactivateProduct;

public sealed class DeactivateProductCommandValidator : AbstractValidator<DeactivateProductCommand>
{
    public DeactivateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}
