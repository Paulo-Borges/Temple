using FluentValidation;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Membros;
using Temple.Domain.Entities;
using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Application.Usuarios;

public sealed class UsuarioService(
    IGerenciadorDeUsuarios gerenciadorDeUsuarios,
    IMembroRepository membroRepository,
    IValidator<CriarUsuarioRequest> criarUsuarioValidator,
    IValidator<AlterarPerfisRequest> alterarPerfisValidator,
    IValidator<VincularMembroRequest> vincularMembroValidator)
{
    public async Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        await criarUsuarioValidator.ValidateAndThrowAsync(request, cancellationToken);

        await GarantirMembroAtivoAsync(request.MembroId, cancellationToken);

        if (await gerenciadorDeUsuarios.ObterIdDoUsuarioDoMembroAsync(request.MembroId, cancellationToken) is Guid)
        {
            throw new ConflitoException("Este membro já está vinculado a outro usuário.");
        }

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

    public async Task<UsuarioResponse> VincularMembroAsync(Guid id, VincularMembroRequest request, CancellationToken cancellationToken)
    {
        await vincularMembroValidator.ValidateAndThrowAsync(request, cancellationToken);

        await ObterExistenteAsync(id, cancellationToken);
        await GarantirMembroAtivoAsync(request.MembroId, cancellationToken);

        Guid? usuarioDoMembro = await gerenciadorDeUsuarios.ObterIdDoUsuarioDoMembroAsync(request.MembroId, cancellationToken);

        if (usuarioDoMembro is Guid outroUsuario && outroUsuario != id)
        {
            throw new ConflitoException("Este membro já está vinculado a outro usuário.");
        }

        return await gerenciadorDeUsuarios.VincularMembroAsync(id, request.MembroId, cancellationToken);
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

    private async Task GarantirMembroAtivoAsync(Guid membroId, CancellationToken cancellationToken)
    {
        Membro? membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);

        if (membro is null)
        {
            throw new NaoEncontradoException($"Membro '{membroId}' não encontrado.");
        }

        if (!membro.Ativo)
        {
            throw new DomainException("Não é possível vincular um usuário a um membro desativado.");
        }
    }
}
