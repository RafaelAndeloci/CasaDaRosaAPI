using CasaDaRosa.Application.Features.Admin.Products.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive) : IRequest<AdminProductResponse>;
