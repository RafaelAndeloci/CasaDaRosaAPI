using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository(CasaDaRosaDbContext dbContext) : ICategoryRepository
{
    public Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        return dbContext.Categories.AddAsync(category, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public Task<Category?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }
}
