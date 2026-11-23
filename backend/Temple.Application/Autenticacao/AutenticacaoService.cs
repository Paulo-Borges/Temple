using FluentValidation;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Membros;
using Temple.Domain.Entities;

namespace Temple.Application.Autenticacao;

public sealed class AutenticacaoService(
    IServicoDeIdentidade servicoDeIdentidade,
    IGeradorDeToken geradorDeToken,
     IMembroRepository membroRepository,
    IValidator<LoginRequest> loginValidator)
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        UsuarioAutenticado? usuario = await servicoDeIdentidade.ValidarCredenciaisAsync(request.Email, request.Senha, cancellationToken);

        if (usuario is null || await TemMembroDesativadoAsync(usuario, cancellationToken))
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

                    return new UsuarioLogadoResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.MembroId, usuario.Perfis);


    }

        private async Task<bool> TemMembroDesativadoAsync(UsuarioAutenticado usuario, CancellationToken cancellationToken)
    {
        if (usuario.MembroId is not Guid membroId)
        {
            return false;
        }

        Membro? membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);

        return membro is null || !membro.Ativo;
    }
}
