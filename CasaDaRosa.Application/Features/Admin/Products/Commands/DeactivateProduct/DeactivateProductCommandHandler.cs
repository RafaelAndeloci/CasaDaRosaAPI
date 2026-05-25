using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.DeactivateProduct;

public sealed class DeactivateProductCommandHandler(
    IUserContext userContext,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeactivateProductCommand>
{
    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var product = await productRepository.GetTrackedByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundApplicationException("products.not_found", "Product not found.");
        }

        product.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
