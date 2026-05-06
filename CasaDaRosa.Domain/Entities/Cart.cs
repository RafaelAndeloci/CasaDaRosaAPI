using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Enums;

namespace CasaDaRosa.Domain.Entities;

public class Cart : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public CartStatus Status { get; private set; } = CartStatus.Active;

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart()
    {
    }

    public Cart(Guid userId)
    {
        UserId = userId;
    }
}
