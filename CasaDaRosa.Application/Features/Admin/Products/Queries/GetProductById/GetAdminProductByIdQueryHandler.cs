using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Products.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Queries.GetProductById;

public sealed class GetAdminProductByIdQueryHandler(
    IUserContext userContext,
    IProductRepository productRepository) : IRequestHandler<GetAdminProductByIdQuery, AdminProductResponse>
{
    public async Task<AdminProductResponse> Handle(GetAdminProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundApplicationException("products.not_found", "Product not found.");
        }

        return AdminProductResponseMapper.ToResponse(product);
    }
}
