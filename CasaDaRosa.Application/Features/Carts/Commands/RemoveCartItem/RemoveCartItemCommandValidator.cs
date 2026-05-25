using FluentValidation;

namespace CasaDaRosa.Application.Features.Carts.Commands.RemoveCartItem;

public sealed class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty();
    }
}
