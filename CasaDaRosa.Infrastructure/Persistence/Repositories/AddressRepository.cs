using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Domain.Entities.Addresses;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Persistence.Repositories;

public sealed class AddressRepository(CasaDaRosaDbContext dbContext) : IAddressRepository
{
    public async Task<IReadOnlyCollection<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Addresses
            .AsNoTracking()
            .Where(address => address.UserId == userId)
            .OrderByDescending(address => address.IsDefault)
            .ThenBy(address => address.City)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Address address, CancellationToken cancellationToken = default)
    {
        await dbContext.Addresses.AddAsync(address, cancellationToken);
    }
}
