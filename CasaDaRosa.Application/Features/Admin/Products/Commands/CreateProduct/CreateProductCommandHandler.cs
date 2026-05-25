using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Products.Common;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.ValueObjects;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, AdminProductResponse>
{
    public async Task<AdminProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
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

        var product = Product.Create(
            request.CategoryId,
            request.Name,
            request.Description,
            new Money(request.Price, Currency.Brl),
            request.StockQuantity);

        if (!request.IsActive)
        {
            product.Deactivate();
        }

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdminProductResponseMapper.ToResponse(product);
    }
}
