using CasaDaRosa.Domain.Entities.Products;

namespace CasaDaRosa.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
