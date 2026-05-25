using FluentValidation;

namespace CasaDaRosa.Application.Features.Addresses.Commands.CreateAddress;

public sealed class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Number)
            .GreaterThan((short)0);

        RuleFor(x => x.Neighborhood)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(2);

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Complement)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Complement));

        RuleFor(x => x.Reference)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Reference));
    }
}
