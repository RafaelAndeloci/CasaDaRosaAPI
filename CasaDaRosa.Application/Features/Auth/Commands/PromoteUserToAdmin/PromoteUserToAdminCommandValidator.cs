using FluentValidation;

namespace CasaDaRosa.Application.Features.Auth.Commands.PromoteUserToAdmin;

public sealed class PromoteUserToAdminCommandValidator : AbstractValidator<PromoteUserToAdminCommand>
{
    public PromoteUserToAdminCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
