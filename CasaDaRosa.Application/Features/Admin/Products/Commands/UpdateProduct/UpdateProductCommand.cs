using CasaDaRosa.Application.Features.Admin.Products.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive) : IRequest<AdminProductResponse>;
