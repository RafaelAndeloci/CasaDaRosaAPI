using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid ProductId) : IRequest;
