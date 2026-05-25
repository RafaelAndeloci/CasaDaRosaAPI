using FluentValidation;

namespace CasaDaRosa.Application.Features.Orders.Commands.CheckoutOrder;

public sealed class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
{
    public CheckoutOrderCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .NotEmpty();

        RuleFor(x => x.PaymentMethodId)
            .InclusiveBetween(1, 3);

        RuleFor(x => x.DeliveryAvailableFromUtc)
            .Must(value => value > DateTime.UtcNow)
            .WithMessage("Delivery availability must be in the future.");
    }
}
