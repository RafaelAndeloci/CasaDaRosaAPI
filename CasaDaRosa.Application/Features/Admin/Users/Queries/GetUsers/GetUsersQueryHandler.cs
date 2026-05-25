using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Filters;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Users.Common;
using CasaDaRosa.Domain.Entities.Users;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(
    IUserContext userContext,
    IUserRepository userRepository) : IRequestHandler<GetUsersQuery, PagedResult<AdminUserListItemResponse>>
{
    public async Task<PagedResult<AdminUserListItemResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var users = await userRepository.GetAllAsync(cancellationToken);

        var filteredUsers = users
            .Where(user => request.RoleId is null || (int)user.Role == request.RoleId)
            .Where(user => request.StatusId is null || (int)user.Status == request.StatusId)
            .Where(user =>
                TextFilterUtility.ContainsNormalized(user.Name.ToString(), request.Search)
                || TextFilterUtility.ContainsNormalized(user.Email.ToString(), request.Search))
            .OrderBy(user => user.Name.ToString())
            .ThenBy(user => user.Email.ToString())
            .ToArray();

        var totalCount = filteredUsers.Length;

        var pagedUsers = filteredUsers
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(AdminUserResponseMapper.ToListItem)
            .ToArray();

        return PagedResult<AdminUserListItemResponse>.Create(pagedUsers, request.PageNumber, request.PageSize, totalCount);
    }
}
