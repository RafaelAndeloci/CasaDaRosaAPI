using CasaDaRosa.Domain.Entities.Categories;

namespace CasaDaRosa.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
