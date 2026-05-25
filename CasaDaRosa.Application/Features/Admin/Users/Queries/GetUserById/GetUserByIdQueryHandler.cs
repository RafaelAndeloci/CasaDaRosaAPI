using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Application.Features.Admin.Users.Common;
using MediatR;

namespace CasaDaRosa.Application.Features.Admin.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(
    IUserContext userContext,
    IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, AdminUserResponse>
{
    public async Task<AdminUserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!userContext.IsAdmin)
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundApplicationException("users.not_found", "User not found.");
        }

        return AdminUserResponseMapper.ToResponse(user);
    }
}
