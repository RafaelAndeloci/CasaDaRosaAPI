using FluentValidation;

namespace CasaDaRosa.Application.Features.Admin.Users.Queries.GetUsers;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.RoleId)
            .InclusiveBetween(1, 2)
            .When(x => x.RoleId.HasValue);

        RuleFor(x => x.StatusId)
            .InclusiveBetween(0, 2)
            .When(x => x.StatusId.HasValue);
    }
}
