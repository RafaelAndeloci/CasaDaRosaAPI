using CasaDaRosa.Application.Common.Responses;
using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Application.Features.Orders.Common;

public static class OrderResponseMapper
{
    public static OrderResponse FromOrder(Order order)
    {
        var items = order.Items
            .Select(item => new OrderItemResponse(
                item.Id,
                item.ProductId,
                item.ProductNameSnapshot,
                item.Quantity,
                item.UnitPrice.Amount,
                item.Total.Amount,
                NormalizeCurrencyCode(item.Total.Currency ?? item.UnitPrice.Currency)))
            .ToArray();

        return new OrderResponse(
            order.Id,
            order.AddressId,
            EnumValueResponse.FromEnum(order.PaymentMethod),
            EnumValueResponse.FromEnum(order.Status),
            order.DeliveryAvailableFromUtc,
            order.TotalAmount.Amount,
            NormalizeCurrencyCode(order.TotalAmount.Currency),
            items);
    }

    private static string? NormalizeCurrencyCode(Currency? currency)
    {
        return string.IsNullOrWhiteSpace(currency?.Code) ? null : currency.Code;
    }
}
