using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Filters;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Products.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Queries.GetProducts;

public sealed class GetAdminProductsQueryHandler(
    IUserContext userContext,
    IProductRepository productRepository) : IRequestHandler<GetAdminProductsQuery, PagedResult<AdminProductResponse>>
{
    public async Task<PagedResult<AdminProductResponse>> Handle(GetAdminProductsQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var products = await productRepository.GetAllAsync(cancellationToken);

        var filteredProducts = products
            .Where(product => request.CategoryId is null || product.CategoryId == request.CategoryId)
            .Where(product => request.IsActive is null || product.IsActive == request.IsActive.Value)
            .Where(product => TextFilterUtility.ContainsNormalized(product.Name.ToString(), request.Name))
            .OrderBy(product => product.Name.ToString())
            .ToArray();

        var totalCount = filteredProducts.Length;

        var pagedProducts = filteredProducts
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(AdminProductResponseMapper.ToResponse)
            .ToArray();

        return PagedResult<AdminProductResponse>.Create(pagedProducts, request.PageNumber, request.PageSize, totalCount);
    }
}
