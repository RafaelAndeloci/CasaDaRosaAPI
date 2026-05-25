using FluentValidation;

namespace CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;

public sealed class GetMyAddressesQueryValidator : AbstractValidator<GetMyAddressesQuery>
{
    public GetMyAddressesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.State)
            .MaximumLength(2)
            .When(x => !string.IsNullOrWhiteSpace(x.State));
    }
}
