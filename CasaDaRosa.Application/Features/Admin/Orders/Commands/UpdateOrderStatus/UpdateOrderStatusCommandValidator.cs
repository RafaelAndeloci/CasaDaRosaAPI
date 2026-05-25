using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Orders.Commands.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.StatusId)
            .InclusiveBetween(1, 6);
    }
}
