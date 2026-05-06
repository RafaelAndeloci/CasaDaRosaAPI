using MediatR;

namespace CasaDaRosa.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductListItemResponse>>
{
    public Task<IReadOnlyCollection<ProductListItemResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<ProductListItemResponse> products = [];
        return Task.FromResult(products);
    }
}
