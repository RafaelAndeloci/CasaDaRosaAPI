using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Domain.Entities.Carts;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Persistence.Repositories;

public sealed class CartRepository(CasaDaRosaDbContext dbContext) : ICartRepository
{
    public Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return GetByUserIdAsync(userId, includeItems: false, cancellationToken);
    }

    public Task<Cart?> GetByUserIdAsync(Guid userId, bool includeItems, CancellationToken cancellationToken = default)
    {
        IQueryable<Cart> query = dbContext.Carts;

        if (includeItems)
        {
            query = query.Include(cart => cart.Items);
        }

        return query.FirstOrDefaultAsync(cart => cart.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await dbContext.Carts.AddAsync(cart, cancellationToken);
    }
}
