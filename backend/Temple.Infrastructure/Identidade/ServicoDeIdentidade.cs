using Microsoft.AspNetCore.Identity;
using Temple.Application.Autenticacao;

namespace Temple.Infrastructure.Identidade;

internal sealed class ServicoDeIdentidade(UserManager<Usuario> userManager) : IServicoDeIdentidade
{
    public async Task<UsuarioAutenticado?> ValidarCredenciaisAsync(string email, string senha, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Usuario? usuario = await userManager.FindByEmailAsync(email);

        if (usuario is null)
        {
            return null;
        }

        if (await userManager.IsLockedOutAsync(usuario))
        {
            return null;
        }

        if (!await userManager.CheckPasswordAsync(usuario, senha))
        {
            await userManager.AccessFailedAsync(usuario);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(usuario);

        return ParaUsuarioAutenticado(usuario);
    }

    public async Task<UsuarioAutenticado?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Usuario? usuario = await userManager.FindByIdAsync(id.ToString());

        if (usuario is null)
        {
            return null;
        }

        return ParaUsuarioAutenticado(usuario);
    }

    private static UsuarioAutenticado ParaUsuarioAutenticado(Usuario usuario)
    {
        if (usuario.Email is not string email)
        {
            throw new InvalidOperationException($"O usuário '{usuario.Id}' está sem e-mail.");
        }

        return new UsuarioAutenticado(usuario.Id, usuario.Nome, email);
    }
}
