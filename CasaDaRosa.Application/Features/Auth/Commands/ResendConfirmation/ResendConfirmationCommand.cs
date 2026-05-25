using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.ResendConfirmation;

public sealed record ResendConfirmationCommand(string Email) : IRequest;
