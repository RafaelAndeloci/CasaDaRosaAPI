using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Carts.Services;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Services;

public sealed class CartProductEligibilityService(CasaDaRosaDbContext dbContext) : ICartProductEligibilityService
{
    public Result ValidateProductEligibility(Guid productId, int desiredQuantity)
    {
        var product = dbContext.Products
            .AsNoTracking()
            .FirstOrDefault(product => product.Id == productId);

        if (product is null)
        {
            return Result.Failure(new Error("cart.product.not_found", "Product not found."));
        }

        if (!product.IsActive)
        {
            return Result.Failure(new Error("cart.product.inactive", "Product is inactive."));
        }

        if (product.StockQuantity.Value < desiredQuantity)
        {
            return Result.Failure(new Error("cart.product.insufficient_stock", "Insufficient stock for the requested quantity."));
        }

        return Result.Success();
    }
}
