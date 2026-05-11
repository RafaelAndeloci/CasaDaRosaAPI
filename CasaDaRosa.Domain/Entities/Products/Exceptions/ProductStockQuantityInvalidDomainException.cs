using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products.Exceptions;

public sealed class ProductStockQuantityInvalidDomainException()
    : DomainValidationException("Product.InvalidStockQuantity", "Product stock cannot be negative.");
