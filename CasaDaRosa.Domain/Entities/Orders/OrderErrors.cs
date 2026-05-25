using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Orders;

public static class OrderErrors
{
    public static Error NotPending = new(
        "Order.NotPending", 
        "The order is not pending and cannot be confirmed.");

    public static Error NotConfirmed = new(
        "Order.NotConfirmed",
        "The order is not confirmed and cannot be cancelled.");

    public static Error InvalidStatusTransition = new(
        "Order.InvalidStatusTransition",
        "The requested order status transition is not allowed.");
}