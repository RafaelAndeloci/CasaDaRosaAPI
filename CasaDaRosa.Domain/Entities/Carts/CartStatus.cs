namespace CasaDaRosa.Domain.Entities.Carts;

public enum CartStatus
{
    // No items
    Empty = 1,
    // When has at least 1 item
    Active = 2,
    // When the user has completed the checkout process, but the order has not been created yet
    Abandoned = 3
}
