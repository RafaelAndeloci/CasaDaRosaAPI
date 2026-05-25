using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Orders.Queries.GetOrders;

public sealed class GetAdminOrdersQueryValidator : AbstractValidator<GetAdminOrdersQuery>
{
    public GetAdminOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.StatusId)
            .InclusiveBetween(1, 6)
            .When(x => x.StatusId.HasValue);

        RuleFor(x => x.PaymentMethodId)
            .InclusiveBetween(1, 3)
            .When(x => x.PaymentMethodId.HasValue);
    }
}
