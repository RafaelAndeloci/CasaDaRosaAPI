using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Carts.Exceptions;
using CasaDaRosa.Domain.Entities.Carts.Services;

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

        UpdateStatusBasedOnItems();
    }

    public static Cart Create(Guid userId, CartStatus status, List<CartItem> items)
    {
        if (userId == Guid.Empty)
        {
            throw new CartUserRequiredException();
        }

        if (items is null)
        {
            throw new CartItemsCollectionRequiredException();
        }

        return new(
            id: Guid.NewGuid(),
            userId: userId,
            status: status,
            items: items);
    }

    public Result AddItem(CartItem item, ICartProductEligibilityService productEligibilityService)
    {
        if (item is null)
        {
            throw new CartItemRequiredException();
        }

        if (productEligibilityService is null)
        {
            throw new CartProductEligibilityServiceRequiredException();
        }

        if (Status == CartStatus.Abandoned)
        {
            return Result.Failure(CartErrors.NotActive);
        }

        if (_items.Any(i => i.ProductId == item.ProductId))
        {
            return Result.Failure(CartErrors.DuplicatedItem);
        }

        var eligibilityResult = productEligibilityService.ValidateProductEligibility(item.ProductId, item.Quantity);

        if (eligibilityResult.IsFailure)
        {
            return eligibilityResult;
        }

        _items.Add(item);
        UpdateStatusBasedOnItems();
        Touch();
        return Result.Success();
    }

    public Result RemoveItem(Guid itemId)
    {
        if (Status == CartStatus.Abandoned)
        {
            return Result.Failure(CartErrors.NotActive);
        }

        var item = _items.FirstOrDefault(i => i.Id == itemId);

        if (item == null)
        {
            return Result.Failure(CartErrors.ItemNotInCart);
        }

        _items.Remove(item);
        UpdateStatusBasedOnItems();
        Touch();
        return Result.Success();
    }

    public Result ChangeProductQuantity(Guid productId, int quantity, ICartProductEligibilityService productEligibilityService)
    {
        if (productEligibilityService is null)
        {
            throw new CartProductEligibilityServiceRequiredException();
        }

        if (Status != CartStatus.Active)
        {
            return Result.Failure(CartErrors.NotActive);
        }

        var productInCart = _items.FirstOrDefault(i => i.ProductId == productId);

        if (productInCart == null)
        {
            return Result.Failure(CartErrors.ProductNotInCart);
        }

        var eligibilityResult = productEligibilityService.ValidateProductEligibility(productId, quantity);

        if (eligibilityResult.IsFailure)
        {
            return eligibilityResult;
        }

        var result = productInCart.UpdateQuantity(quantity);

        if (result.IsFailure)
        {
            return result;
        }

        Touch();
        return result;
    }

    public Result ClearItems()
    {
        if (Status != CartStatus.Active)
        {
            return Result.Failure(CartErrors.NotActive);
        }

        _items.Clear();
        Status = CartStatus.Empty;
        Touch();
        return Result.Success();
    }

    public Result Abandon()
    {
        if(Status == CartStatus.Abandoned)
        {
            return Result.Failure(CartErrors.AlreadyAbandoned);
        }

        if (Status != CartStatus.Active)
        {
            return Result.Failure(CartErrors.NotActive);
        }

        Status = CartStatus.Abandoned;
        Touch();
        return Result.Success();
    }

    private void UpdateStatusBasedOnItems()
    {
        if (Status == CartStatus.Abandoned)
        {
            return;
        }

        Status = _items.Count == 0
            ? CartStatus.Empty
            : CartStatus.Active;
    }
}