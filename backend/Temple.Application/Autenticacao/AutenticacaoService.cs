using FluentValidation;
using Temple.Application.Comum.Excecoes;

namespace Temple.Application.Autenticacao;

public sealed class AutenticacaoService(
    IServicoDeIdentidade servicoDeIdentidade,
    IGeradorDeToken geradorDeToken,
    IValidator<LoginRequest> loginValidator)
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        UsuarioAutenticado? usuario = await servicoDeIdentidade.ValidarCredenciaisAsync(request.Email, request.Senha, cancellationToken);

        if (usuario is null)
        {
            throw new NaoAutorizadoException("E-mail ou senha inválidos.");
        }

        return geradorDeToken.Gerar(usuario);
    }

    public async Task<UsuarioLogadoResponse> ObterUsuarioLogadoAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        UsuarioAutenticado? usuario = await servicoDeIdentidade.ObterPorIdAsync(usuarioId, cancellationToken);

        if (usuario is null)
        {
            throw new NaoAutorizadoException("O usuário do token não existe mais.");
        }

                return new UsuarioLogadoResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfis);

    }
}
