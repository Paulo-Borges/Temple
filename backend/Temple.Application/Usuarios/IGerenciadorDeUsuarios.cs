using Temple.Domain.Enums;

namespace Temple.Application.Usuarios;

public interface IGerenciadorDeUsuarios
{
    Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken);

    Task<UsuarioResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken cancellationToken);

    Task<int> ContarComPerfilAsync(Perfil perfil, CancellationToken cancellationToken);

    Task<UsuarioResponse> AlterarPerfisAsync(Guid id, IReadOnlyList<Perfil> perfis, CancellationToken cancellationToken);
      Task<Guid?> ObterIdDoUsuarioDoMembroAsync(Guid membroId, CancellationToken cancellationToken);

    Task<UsuarioResponse> VincularMembroAsync(Guid id, Guid membroId, CancellationToken cancellationToken);
}
