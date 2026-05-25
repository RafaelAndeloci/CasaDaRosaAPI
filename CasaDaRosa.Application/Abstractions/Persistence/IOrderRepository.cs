using CasaDaRosa.Domain.Entities.Orders;

namespace CasaDaRosa.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
}
