using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Carts.Services;

public interface ICartProductEligibilityService
{
    Result ValidateProductEligibility(Guid productId, int desiredQuantity);
}
