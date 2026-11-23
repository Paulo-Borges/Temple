using Temple.Application.Comum.Acesso;

namespace Temple.Api.Autorizacao;

internal sealed class UsuarioAtual(IHttpContextAccessor httpContextAccessor) : IUsuarioAtual
{
    public Guid Id
    {
        get
        {
            if (httpContextAccessor.HttpContext is not HttpContext contexto)
            {
                throw new InvalidOperationException("Não há uma requisição HTTP em andamento.");
            }

            return contexto.User.ObterIdDoUsuario();
        }
    }
}