using Temple.Domain.Enums;

namespace Temple.Application.Usuarios;

public sealed record UsuarioResponse(
    Guid Id,
    string Nome,
    string Email,
    Guid? MembroId,
    IReadOnlyList<Perfil> Perfis);
