using CasaDaRosa.Domain.Entities.Categories;
using FluentAssertions;

namespace CasaDaRosa.Domain.UnitTests.Entities.Categories;

public class CategoryTests
{
    [Fact]
    public void Create_ShouldInitializeCategoryWithProvidedValues()
    {
        var category = Category.Create("Rosas", "Flores vermelhas", true);

        category.Name.ToString().Should().Be("Rosas");
        category.Description!.ToString().Should().Be("Flores vermelhas");
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_ShouldChangeNameAndDescription()
    {
        var category = Category.Create("Rosas", "Flores vermelhas", true);

        category.UpdateDetails("Orquídeas", "Flores delicadas");

        category.Name.ToString().Should().Be("Orquídeas");
        category.Description!.ToString().Should().Be("Flores delicadas");
        category.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_AndActivate_ShouldToggleIsActive()
    {
        var category = Category.Create("Rosas", null, true);

        category.Deactivate();
        category.IsActive.Should().BeFalse();

        category.Activate();
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithBlankName_ShouldThrow()
    {
        var action = () => Category.Create("   ", null, true);

        action.Should().Throw<CasaDaRosa.Domain.Entities.Categories.Exceptions.CategoryNameRequiredException>();
    }
}
