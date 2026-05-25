using CasaDaRosa.Application.Abstractions.Auth;

namespace CasaDaRosa.Application.Abstractions;

public interface IAuthEmailService
{
    Task SendEmailConfirmationAsync(SendEmailConfirmationRequest request, CancellationToken cancellationToken = default);
}
