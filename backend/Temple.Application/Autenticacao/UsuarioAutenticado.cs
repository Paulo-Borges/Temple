using Temple.Domain.Enums;

namespace Temple.Application.Autenticacao;

public sealed record UsuarioAutenticado(
    Guid Id,
    string Nome,
    string Email,
    IReadOnlyList<Perfil> Perfis);
