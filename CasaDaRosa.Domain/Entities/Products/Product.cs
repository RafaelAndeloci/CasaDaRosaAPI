using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.Entities.Products.Exceptions;
using CasaDaRosa.Domain.Exceptions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities.Products;

public class Product : AuditableEntity, IAggregateRoot
{
    public Guid CategoryId { get; private set; }
    public ProductName Name { get; private set; } = null!;
    public ProductDescription? Description { get; private set; }
    public Money Price { get; private set; } = null!;
    public StockQuantity StockQuantity { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private Product() : base(Guid.Empty)
    {
    }

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
        if (categoryId == Guid.Empty)
        {
            throw new ProductCategoryRequiredException();
        }

        var stockQuantityResult = StockQuantity.Create(stockQuantity);

        if (stockQuantityResult.IsFailure)
        {
            throw new ProductStockQuantityInvalidDomainException();
        }

        var product = new Product(
            id: Guid.NewGuid(),
            categoryId: categoryId,
            ProductName.Create(name),
            string.IsNullOrWhiteSpace(description) ? null : ProductDescription.Create(description),
            price,
            stockQuantityResult.Value,
            isActive: true,
            reviews: []
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
        if (review is null)
        {
            throw new ProductReviewRequiredException();
        }

        if(_reviews.Any(r => r.Id == review.Id))
        {
            return Result.Failure(ProductErrors.DuplicatedReview);
        }

        _reviews.Add(review);
        Touch();

        return Result.Success();
    }
}