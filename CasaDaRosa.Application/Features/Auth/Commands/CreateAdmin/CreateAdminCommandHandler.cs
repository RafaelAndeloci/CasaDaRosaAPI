using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Domain.Entities.Users;
using MediatR;

namespace CasaDaRosa.Application.Features.Auth.Commands.CreateAdmin;

public sealed class CreateAdminCommandHandler(
    IUserContext userContext,
    IUserRepository userRepository,
    ISecurityService securityService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateAdminCommand, CreateAdminResponse>
{
    public async Task<CreateAdminResponse> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var currentUser = await userRepository.GetByIdAsync(userContext.UserId.Value, cancellationToken);

        if (currentUser is null)
        {
            throw new UnauthorizedApplicationException();
        }

        if (!currentUser.IsAdmin())
        {
            throw new ForbiddenApplicationException("auth.admin_required", "Administrator access is required.");
        }

        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictApplicationException("auth.email_already_in_use", "A user with the provided email already exists.");
        }

        var passwordHash = securityService.HashPassword(request.Password);
        var admin = User.CreateAdmin(request.FullName, request.Email, passwordHash, request.PhoneNumber);

        await userRepository.AddAsync(admin, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAdminResponse(admin.Id);
    }
}
