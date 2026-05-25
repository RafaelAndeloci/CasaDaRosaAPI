using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.ActivateProduct;

public sealed record ActivateProductCommand(Guid ProductId) : IRequest;
