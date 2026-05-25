using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid CategoryId) : IRequest;
