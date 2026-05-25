using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(CasaDaRosaDbContext dbContext) : IProductRepository
{
    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        return dbContext.Products.AddAsync(product, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }
}
