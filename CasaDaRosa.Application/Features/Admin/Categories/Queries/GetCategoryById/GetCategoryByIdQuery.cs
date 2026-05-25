using CasaDaRosa.Application.Features.Admin.Categories.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IRequest<AdminCategoryResponse>;
