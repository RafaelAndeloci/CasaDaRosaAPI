using CasaDaRosa.Application.Abstractions.Persistence;
using MediatR;

namespace CasaDaRosa.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductDetailsResponse?>
{
    public async Task<ProductDetailsResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        return new ProductDetailsResponse(
            product.Id,
            product.Name.ToString(),
            product.Description?.ToString(),
            product.Price.Amount,
            product.StockQuantity.Value,
            product.CategoryId,
            product.IsActive);
    }
}
