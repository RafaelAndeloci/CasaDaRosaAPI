using CasaDaRosa.Application.Common.Responses;
using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Application.Features.Carts.Common;

public static class CartResponseMapper
{
    public static CartResponse FromCart(Cart cart)
    {
        var items = cart.Items
            .Select(item => new CartItemResponse(
                item.Id,
                item.ProductId,
                item.Quantity,
                item.UnitPrice.Amount,
                item.UnitPrice.Amount * item.Quantity,
                NormalizeCurrencyCode(item.UnitPrice.Currency)))
            .ToArray();

        var totalAmount = items.Sum(item => item.TotalPrice);
        var currencyCode = items.FirstOrDefault()?.CurrencyCode;

        return new CartResponse(
            cart.Id,
            EnumValueResponse.FromEnum(cart.Status),
            items,
            totalAmount,
            currencyCode);
    }

    public static CartResponse Empty()
    {
        return new CartResponse(
            null,
            EnumValueResponse.FromEnum(CartStatus.Empty),
            [],
            0m,
            null);
    }

    private static string? NormalizeCurrencyCode(Currency? currency)
    {
        return string.IsNullOrWhiteSpace(currency?.Code) ? null : currency.Code;
    }
}
