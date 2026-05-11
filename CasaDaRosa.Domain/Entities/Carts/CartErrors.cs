using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Carts;

public static class CartErrors
{
    public static Error NotActive = new(
        "Cart.NotActive", 
        "The cart is not active.");
    public static Error ProductNotInCart = new(
        "Cart.ProductNotInCart",
        "The specified product is not in the cart.");
    public static Error DuplicatedItem = new(
        "Cart.DuplicatedItem",
        "The specified product is already in the cart.");
    public static Error InvalidQuantity = new(
        "Cart.InvalidQuantity",
        "The specified quantity is invalid. Quantity must be greater than zero.");
    public static Error ItemNotInCart = new(
        "Cart.ItemNotInCart",
        "The specified item is not in the cart.");

    public static Error AlreadyAbandoned = new(
        "Cart.AlreadyAbandoned",
        "The cart has already been abandoned.");
}