using CasaDaRosa.Application.Features.Auth.Common;

namespace CasaDaRosa.Application.Features.Auth.Commands.Login;

public sealed record LoginResponse(AuthResponse Auth);
