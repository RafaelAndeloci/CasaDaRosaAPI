using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Orders.Common;
using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.ValueObjects;
using MediatR;

namespace CasaDaRosa.Application.Features.Orders.Commands.CheckoutOrder;

public sealed class CheckoutOrderCommandHandler(
    IUserContext userContext,
    ICartRepository cartRepository,
    IAddressRepository addressRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CheckoutOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var cart = await cartRepository.GetByUserIdAsync(userContext.UserId.Value, includeItems: true, cancellationToken: cancellationToken);

        if (cart is null)
        {
            throw new NotFoundApplicationException("cart.not_found", "Cart not found.");
        }

        if (!cart.Items.Any())
        {
            throw new UnprocessableApplicationException("orders.checkout.empty_cart", "Cart must contain at least one item to perform checkout.");
        }

        var addresses = await addressRepository.GetByUserIdAsync(userContext.UserId.Value, cancellationToken);
        var selectedAddress = addresses.FirstOrDefault(address => address.Id == request.AddressId);

        if (selectedAddress is null)
        {
            throw new NotFoundApplicationException("addresses.not_found", "Address not found for the authenticated user.");
        }

        var paymentMethod = (PaymentMethod)request.PaymentMethodId;
        var order = Order.Create(userContext.UserId.Value, selectedAddress.Id, paymentMethod, request.DeliveryAvailableFromUtc);

        foreach (var cartItem in cart.Items)
        {
            var product = await productRepository.GetByIdAsync(cartItem.ProductId, cancellationToken);

            if (product is null)
            {
                throw new NotFoundApplicationException("products.not_found", "Product not found.");
            }

            var orderItem = OrderItem.Create(
                order.Id,
                cartItem.ProductId,
                product.Name.ToString(),
                cartItem.Quantity,
                cartItem.UnitPrice);

            order.AddItem(orderItem);
        }

        var confirmationResult = order.Confirm();

        if (confirmationResult.IsFailure)
        {
            throw new UnprocessableApplicationException(confirmationResult.Error.Code, confirmationResult.Error.Name);
        }

        var clearCartResult = cart.ClearItems();

        if (clearCartResult.IsFailure)
        {
            throw new UnprocessableApplicationException(clearCartResult.Error.Code, clearCartResult.Error.Name);
        }

        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderResponseMapper.FromOrder(order);
    }
}
