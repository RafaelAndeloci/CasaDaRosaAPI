using System.Runtime.CompilerServices;
using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Exceptions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities.Products;

public class Product : AuditableEntity, IAggregateRoot
{
    public Guid CategoryId { get; private set; }
    public ProductName Name { get; private set; }
    public ProductDescription? Description { get; private set; }
    public Money Price { get; private set; }
    public StockQuantity StockQuantity { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private Product(
        Guid id,
        Guid categoryId,
        ProductName name,
        ProductDescription? description,
        Money price,
        StockQuantity stockQuantity,
        bool isActive,
        List<Review> reviews
        ) : base(id)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        IsActive = isActive;
        _reviews = reviews;
    }

    public static Product Create(Guid categoryId, string name, string? description, Money price, int stockQuantity)
    {
        var product = new Product(
            categoryId,
            ProductName.Create(name),
            string.IsNullOrWhiteSpace(description) ? null : ProductDescription.Create(description),
            price,
            StockQuantity.Create(stockQuantity),
            true
        );
        return product;
    }

    public Result UpdateStockQuantity(int newStockQuantity)
    {
        var result = StockQuantity.Create(newStockQuantity);
        if (!result.IsSuccess) return result;

        StockQuantity = result.Value;
        Touch();
        return Result.Success();
    }

    public Result AddReview(Review review)
    {
        if(_reviews.Any(r => r.Id == review.Id))
        {
            return Result.Failure(ProductErrors.DuplicatedReview);
        }

        _reviews.Add(review);
        return Result.Success();
    }
}