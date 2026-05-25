using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Auth;
using Microsoft.Extensions.Logging;

namespace CasaDaRosa.Infrastructure.Notifications;

public sealed class NoOpAuthEmailService(ILogger<NoOpAuthEmailService> logger) : IAuthEmailService
{
    public Task SendEmailConfirmationAsync(SendEmailConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Email confirmation requested for user {UserId} ({Email}). Token: {ConfirmationToken}",
            request.UserId,
            request.Email,
            request.ConfirmationToken);

        return Task.CompletedTask;
    }
}
