using CasaDaRosa.Domain.Entities.Carts;

namespace CasaDaRosa.Application.Abstractions.Persistence;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cart?> GetByUserIdAsync(Guid userId, bool includeItems, CancellationToken cancellationToken = default);
    Task AddAsync(Cart cart, CancellationToken cancellationToken = default);
}
