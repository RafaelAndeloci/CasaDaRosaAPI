using CasaDaRosa.Domain.Abstractions;
using CasaDaRosa.Domain.ValueObjects;

namespace CasaDaRosa.Domain.Entities;

public class Category : AuditableEntity, IAggregateRoot
{
    public CategoryName Name { get; private set; } = null!;
    public CategoryDescription? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Category()
    {
    }

    public Category(string name, string? description)
    {
        UpdateDetails(CategoryName.Create(name), string.IsNullOrWhiteSpace(description) ? null : CategoryDescription.Create(description));
    }

    public void UpdateDetails(CategoryName name, CategoryDescription? description)
    {
        Name = name;
        Description = description;
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

}
