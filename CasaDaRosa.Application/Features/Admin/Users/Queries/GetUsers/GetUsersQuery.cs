using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Admin.Users.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
    string? Search = null,
    int? RoleId = null,
    int? StatusId = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<AdminUserListItemResponse>>;
