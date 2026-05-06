using CasaDaRosa.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace CasaDaRosa.Infrastructure.Notifications;

public sealed class NoOpEmailService(ILogger<NoOpEmailService> logger) : IEmailService
{
    public Task SendOrderPlacedAsync(OrderPlacedEmailRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Order placed event received for order {OrderId} and user {UserId}. Total: {TotalAmount}.",
            request.OrderId,
            request.UserId,
            request.TotalAmount);

        return Task.CompletedTask;
    }
}
