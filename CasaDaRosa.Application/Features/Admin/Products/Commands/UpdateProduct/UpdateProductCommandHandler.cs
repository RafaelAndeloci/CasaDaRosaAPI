using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Products.Common;
using CasaDaRosa.Domain.ValueObjects;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, AdminProductResponse>
{
    public async Task<AdminProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundApplicationException("categories.not_found", "Category not found.");
        }

        var product = await productRepository.GetTrackedByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundApplicationException("products.not_found", "Product not found.");
        }

        product.UpdateDetails(
            request.CategoryId,
            request.Name,
            request.Description,
            new Money(request.Price, Currency.Brl));

        var stockUpdateResult = product.UpdateStockQuantity(request.StockQuantity);

        if (stockUpdateResult.IsFailure)
        {
            throw new UnprocessableApplicationException(stockUpdateResult.Error.Code, stockUpdateResult.Error.Name);
        }

        if (request.IsActive)
        {
            product.Activate();
        }
        else
        {
            product.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdminProductResponseMapper.ToResponse(product);
    }
}
