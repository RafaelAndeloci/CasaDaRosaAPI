using CasaDaRosa.Domain.Entities.Addresses;

namespace CasaDaRosa.Application.Abstractions.Persistence;

public interface IAddressRepository
{
    Task<IReadOnlyCollection<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Address address, CancellationToken cancellationToken = default);
}
