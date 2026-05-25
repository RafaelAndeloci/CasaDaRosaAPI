using CasaDaRosa.Application.Features.Admin.Categories.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    bool IsActive) : IRequest<AdminCategoryResponse>;
