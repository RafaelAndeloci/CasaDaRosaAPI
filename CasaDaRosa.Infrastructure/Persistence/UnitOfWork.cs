using CasaDaRosa.Application.Abstractions.Persistence;

namespace CasaDaRosa.Infrastructure.Persistence;

public sealed class UnitOfWork(CasaDaRosaDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
