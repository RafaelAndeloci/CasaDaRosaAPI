using CasaDaRosa.Domain.Abstractions;
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
    public bool IsActive { get; private set; } = true;

    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private Product() { }

    public Product(Guid categoryId, string name, string? description, Money price, int stockQuantity)
    {
        SetCategory(categoryId);
        UpdateDetails(ProductName.Create(name), string.IsNullOrWhiteSpace(description) ? null : ProductDescription.Create(description), price);
        UpdateStock(StockQuantity.Create(stockQuantity));
    }

    public void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new DomainValidationException("product.category.invalid", "Product category is required.");
        }

        CategoryId = categoryId;
        SetUpdatedAtUtc();
    }

    public void UpdateDetails(ProductName name, ProductDescription? description, Money price)
    {
        Name = name;
        Description = description;
        Price = price;
        SetUpdatedAtUtc();
    }

    public void UpdateStock(StockQuantity stockQuantity)
    {
        StockQuantity = stockQuantity;
        SetUpdatedAtUtc();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAtUtc();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAtUtc();
    }

    public Review AddReview(Guid userId, decimal ratingValue, string? comment)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("product.review.user.invalid", "Review user is required.");
        }

        var review = new Review(Id, userId, Rating.Create(ratingValue), comment);
        _reviews.Add(review);
        SetUpdatedAtUtc();

        return review;
    }

}
