using Temple.Domain.Enums;

namespace Temple.Application.Usuarios;

public sealed record CriarUsuarioRequest(
    string Nome,
    string Email,
    string Senha,
    IReadOnlyList<Perfil> Perfis);
