using CasaDaRosa.Application.Features.Auth.Common;

namespace CasaDaRosa.Application.Features.Auth.Commands.Register;

public sealed record RegisterResponse(Guid UserId, AuthResponse? Auth);
