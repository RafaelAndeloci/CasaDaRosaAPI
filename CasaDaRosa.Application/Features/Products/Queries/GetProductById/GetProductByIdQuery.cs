using MediatR;

namespace CasaDaRosa.Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailsResponse?>;
