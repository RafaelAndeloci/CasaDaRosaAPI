namespace CasaDaRosa.Application.Abstractions;

public interface IEmailService
{
    Task SendOrderPlacedAsync(OrderPlacedEmailRequest request, CancellationToken cancellationToken = default);
}
