using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Features.Categories.Queries.GetCategories;
using CasaDaRosa.Domain.Entities.Categories;
using FluentAssertions;
using Moq;

namespace CasaDaRosa.Application.UnitTests.Features.Categories;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFilterAndPaginateCategories()
    {
        var matchingCategory = Category.Create("Rosas", "Categoria ativa", true);
        var otherCategory = Category.Create("Tulipas", "Outra categoria", true);
        var categoryRepository = new Mock<ICategoryRepository>();

        categoryRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { otherCategory, matchingCategory });

        var handler = new GetCategoriesQueryHandler(categoryRepository.Object);

        var response = await handler.Handle(new GetCategoriesQuery(Name: "rosa", PageNumber: 1, PageSize: 10), CancellationToken.None);

        response.TotalCount.Should().Be(1);
        response.Items.Should().ContainSingle();
        response.Items.Single().Name.Should().Be("Rosas");
    }
}
