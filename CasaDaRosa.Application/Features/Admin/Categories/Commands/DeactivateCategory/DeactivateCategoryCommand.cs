using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Commands.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid CategoryId) : IRequest;
