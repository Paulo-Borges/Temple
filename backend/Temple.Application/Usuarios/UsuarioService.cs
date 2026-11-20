using FluentValidation;
using Temple.Application.Comum.Excecoes;
using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Application.Usuarios;

public sealed class UsuarioService(
    IGerenciadorDeUsuarios gerenciadorDeUsuarios,
    IValidator<CriarUsuarioRequest> criarUsuarioValidator,
    IValidator<AlterarPerfisRequest> alterarPerfisValidator)
{
    public async Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        await criarUsuarioValidator.ValidateAndThrowAsync(request, cancellationToken);

        return await gerenciadorDeUsuarios.CriarAsync(request, cancellationToken);
    }

    public async Task<UsuarioResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await ObterExistenteAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken cancellationToken)
    {
        return gerenciadorDeUsuarios.ListarAsync(cancellationToken);
    }

    public async Task<UsuarioResponse> AlterarPerfisAsync(Guid id, AlterarPerfisRequest request, CancellationToken cancellationToken)
    {
        await alterarPerfisValidator.ValidateAndThrowAsync(request, cancellationToken);

        UsuarioResponse usuario = await ObterExistenteAsync(id, cancellationToken);

        bool deixaDeSerPastor = usuario.Perfis.Contains(Perfil.Pastor) && !request.Perfis.Contains(Perfil.Pastor);

        if (deixaDeSerPastor && await gerenciadorDeUsuarios.ContarComPerfilAsync(Perfil.Pastor, cancellationToken) == 1)
        {
            throw new DomainException("O sistema precisa ter pelo menos um usuário com o perfil Pastor.");
        }

        return await gerenciadorDeUsuarios.AlterarPerfisAsync(id, request.Perfis, cancellationToken);
    }

    private async Task<UsuarioResponse> ObterExistenteAsync(Guid id, CancellationToken cancellationToken)
    {
        UsuarioResponse? usuario = await gerenciadorDeUsuarios.ObterPorIdAsync(id, cancellationToken);

        if (usuario is null)
        {
            throw new NaoEncontradoException($"Usuário '{id}' não encontrado.");
        }

        return usuario;
    }
}
