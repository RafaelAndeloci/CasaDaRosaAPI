using System.Reflection.Metadata.Ecma335;
using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Carts;

public class Cart : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public CartStatus Status { get; private set; } = CartStatus.Active;

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart(
        Guid id,
        Guid userId,
        CartStatus status,
        List<CartItem> items) : base(id)
    {
        UserId = userId;
        Status = status;
        _items = items;
    }

    public static Cart Create(Guid userId, CartStatus status, List<CartItem> items)
    {
        return new(
            id: Guid.NewGuid(),
            userId: userId,
            status: status,
            items: items);
    }
}
