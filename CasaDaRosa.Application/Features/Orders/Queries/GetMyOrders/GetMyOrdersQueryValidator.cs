using FluentValidation;

namespace CasaDaRosa.Application.Features.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryValidator : AbstractValidator<GetMyOrdersQuery>
{
    public GetMyOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.StatusId)
            .InclusiveBetween(1, 6)
            .When(x => x.StatusId.HasValue);
    }
}
