using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest;
