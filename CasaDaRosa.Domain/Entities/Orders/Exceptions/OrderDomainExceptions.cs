using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Orders.Exceptions;

public sealed class OrderUserRequiredException()
    : DomainValidationException("order.user.invalid", "Order user is required.");

public sealed class OrderAddressRequiredException()
    : DomainValidationException("order.address.invalid", "Order address is required.");

public sealed class OrderDeliveryWindowInvalidException()
    : DomainValidationException("order.delivery_window.invalid", "Delivery availability must be in the future.");

public sealed class OrderItemRequiredException()
    : DomainValidationException("order.item.required", "Order item is required.");
