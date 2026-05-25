using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrderById;

public sealed class GetAdminOrderByIdQueryValidator : AbstractValidator<GetAdminOrderByIdQuery>
{
    public GetAdminOrderByIdQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
    }
}
