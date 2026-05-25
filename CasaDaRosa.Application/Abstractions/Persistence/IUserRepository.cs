using CasaDaRosa.Domain.Entities.Users;

namespace CasaDaRosa.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetTrackedByEmailAsync(string email, CancellationToken cancellationToken = default);
}
