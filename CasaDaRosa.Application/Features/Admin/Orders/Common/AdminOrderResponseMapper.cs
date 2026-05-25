using CasaDaRosa.Application.Common.Responses;
using CasaDaRosa.Application.Features.Orders.Common;
using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Application.Features.Admin.Orders.Common;

public static class AdminOrderResponseMapper
{
    public static AdminOrderResponse ToResponse(Order order)
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

        return new AdminOrderResponse(
            order.Id,
            order.UserId,
            order.AddressId,
            EnumValueResponse.FromEnum(order.PaymentMethod),
            EnumValueResponse.FromEnum(order.Status),
            order.DeliveryAvailableFromUtc,
            order.TotalAmount.Amount,
            NormalizeCurrencyCode(order.TotalAmount.Currency),
            items,
            order.CreatedAtUtc,
            order.UpdatedAtUtc);
    }

    public static AdminOrderListItemResponse ToListItem(Order order)
    {
        return new AdminOrderListItemResponse(
            order.Id,
            order.UserId,
            order.AddressId,
            EnumValueResponse.FromEnum(order.PaymentMethod),
            EnumValueResponse.FromEnum(order.Status),
            order.DeliveryAvailableFromUtc,
            order.TotalAmount.Amount,
            NormalizeCurrencyCode(order.TotalAmount.Currency),
            order.Items.Count,
            order.CreatedAtUtc,
            order.UpdatedAtUtc);
    }

    private static string? NormalizeCurrencyCode(Currency? currency)
    {
        return string.IsNullOrWhiteSpace(currency?.Code) ? null : currency.Code;
    }
}
