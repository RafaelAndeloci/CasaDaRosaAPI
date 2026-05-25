using CasaDaRosa.Application.Features.Admin.Categories.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Description,
    bool IsActive) : IRequest<AdminCategoryResponse>;
