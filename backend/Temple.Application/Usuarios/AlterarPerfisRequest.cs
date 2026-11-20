using Temple.Domain.Enums;

namespace Temple.Application.Usuarios;

public sealed record AlterarPerfisRequest(
    IReadOnlyList<Perfil> Perfis);
