namespace Temple.Application.Autenticacao;

public interface IServicoDeIdentidade
{
    Task<UsuarioAutenticado?> ValidarCredenciaisAsync(string email, string senha, CancellationToken cancellationToken);

    Task<UsuarioAutenticado?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
}
