using MediatR;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Filters;
using CasaDaRosa.Application.Common.Pagination;

namespace CasaDaRosa.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemResponse>>
{
    public async Task<PagedResult<ProductListItemResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);

        var filteredProducts = products
            .Where(product => request.CategoryId is null || product.CategoryId == request.CategoryId)
            .Where(product => TextFilterUtility.ContainsNormalized(product.Name.ToString(), request.Name))
            .OrderBy(product => product.Name.ToString())
            .ToArray();

        var totalCount = filteredProducts.Length;

        var pagedProducts = filteredProducts
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(product => new ProductListItemResponse(
                product.Id,
                product.Name.ToString(),
                product.Description?.ToString(),
                product.Price.Amount,
                product.StockQuantity.Value,
                product.CategoryId))
            .ToArray();

        return PagedResult<ProductListItemResponse>.Create(pagedProducts, request.PageNumber, request.PageSize, totalCount);
    }
}
