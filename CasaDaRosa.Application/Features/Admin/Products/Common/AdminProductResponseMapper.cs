using CasaDaRosa.Domain.Entities.Products;

namespace CasaDaRosa.Application.Features.Admin.Products.Common;

public static class AdminProductResponseMapper
{
    public static AdminProductResponse ToResponse(Product product)
    {
        return new AdminProductResponse(
            product.Id,
            product.Name.ToString(),
            product.Description?.ToString(),
            product.Price.Amount,
            product.StockQuantity.Value,
            product.CategoryId,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc);
    }
}
