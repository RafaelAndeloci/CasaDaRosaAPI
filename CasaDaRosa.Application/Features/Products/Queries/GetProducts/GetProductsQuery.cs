using MediatR;

namespace CasaDaRosa.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery() : IRequest<IReadOnlyCollection<ProductListItemResponse>>;
