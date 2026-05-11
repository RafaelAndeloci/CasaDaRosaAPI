using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Products;

public static class ProductErrors
{
    public static Error InvalidStockQuantity = new(
        "Product.InvalidStockQuantity",
        "Product stock cannot be negative.");
    public static Error GenericUpdateStockError = new(
        "Product.GenericUpdateStockError",
        "An error occurred while updating the product stock.");
    public static Error DuplicatedReview = new(
        "Product.DuplicatedReview",
        "Cannot be added a duplicated review to a product.");
}

public sealed class InvalidStockQuantityDomainException : DomainValidationException
{
    public InvalidStockQuantityDomainException() 
        : base(
        "Product.InvalidStockQuantity",
        "Product stock cannot be negative.") { }
}   