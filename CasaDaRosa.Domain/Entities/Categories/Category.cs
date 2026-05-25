using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Categories;

public class Category : AuditableEntity, IAggregateRoot
{
    public CategoryName Name { get; private set; } = null!;
    public CategoryDescription? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Category() : base(Guid.Empty)
    {
    }

    private Category(
        Guid id,
        CategoryName name, 
        CategoryDescription? description, 
        bool isActive) : base(id)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }

    public static Category Create(
        string name,
        string? description,
        bool isActive)
    {
        return new(
            id: Guid.NewGuid(),
            name: CategoryName.Create(name),
            description: description != null ? CategoryDescription.Create(description) : null,
            isActive: isActive);
    }

    public void UpdateDetails(string name, string? description)
    {
        Name = CategoryName.Create(name);
        Description = description != null ? CategoryDescription.Create(description) : null;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }
}
