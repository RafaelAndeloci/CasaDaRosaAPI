using CasaDaRosa.Domain.Entities.Categories;

namespace CasaDaRosa.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
