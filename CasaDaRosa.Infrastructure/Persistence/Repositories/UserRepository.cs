using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(CasaDaRosaDbContext dbContext) : IUserRepository
{
    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.AddAsync(user, cancellationToken).AsTask();
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AsNoTracking()
            .Include(user => user.Addresses)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AsNoTracking()
            .Include(user => user.Addresses)
            .FirstOrDefaultAsync(user => user.Email.Value == email, cancellationToken);
    }

    public Task<User?> GetTrackedByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .Include(user => user.Addresses)
            .FirstOrDefaultAsync(user => user.Email.Value == email, cancellationToken);
    }
}
