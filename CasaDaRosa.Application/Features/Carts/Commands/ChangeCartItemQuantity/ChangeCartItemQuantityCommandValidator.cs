using FluentValidation;

namespace CasaDaRosa.Application.Features.Carts.Commands.ChangeCartItemQuantity;

public sealed class ChangeCartItemQuantityCommandValidator : AbstractValidator<ChangeCartItemQuantityCommand>
{
    public ChangeCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
