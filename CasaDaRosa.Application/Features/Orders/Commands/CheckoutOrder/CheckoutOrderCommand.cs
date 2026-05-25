using CasaDaRosa.Application.Features.Orders.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Orders.Commands.CheckoutOrder;

public sealed record CheckoutOrderCommand(
    Guid AddressId,
    int PaymentMethodId,
    DateTime DeliveryAvailableFromUtc) : IRequest<OrderResponse>;
