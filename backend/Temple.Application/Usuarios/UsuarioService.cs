using FluentValidation;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Membros;
using Temple.Domain.Entities;
using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Application.Usuarios;

public sealed class UsuarioService(
    IGerenciadorDeUsuarios gerenciadorDeUsuarios,
    IMembroRepository membroRepository,
    ProvedorDeEscopo provedorDeEscopo,
    IValidator<CriarUsuarioRequest> criarUsuarioValidator,
    IValidator<AlterarPerfisRequest> alterarPerfisValidator,
    IValidator<VincularMembroRequest> vincularMembroValidator)
{
    public async Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        await criarUsuarioValidator.ValidateAndThrowAsync(request, cancellationToken);

        await GarantirMembroVinculavelAsync(request.MembroId, cancellationToken);

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

    public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken cancellationToken)
    {
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        return await gerenciadorDeUsuarios.ListarAsync(escopo, cancellationToken);
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
        await GarantirMembroVinculavelAsync(request.MembroId, cancellationToken);

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

        if (usuario is null || !await EstaNoAlcanceAsync(usuario, cancellationToken))
        {
            throw new NaoEncontradoException($"Usuário '{id}' não encontrado.");
        }

        return usuario;
    }

    private async Task<bool> EstaNoAlcanceAsync(UsuarioResponse usuario, CancellationToken cancellationToken)
    {
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (escopo.Total)
        {
            return true;
        }

        if (usuario.MembroId is not Guid membroId)
        {
            return false;
        }

        Membro? membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);

        return membro is not null && escopo.AlcancaMembro(membro);
    }

    private async Task GarantirMembroVinculavelAsync(Guid membroId, CancellationToken cancellationToken)
    {
        Membro? membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (membro is null || !escopo.AlcancaMembro(membro))
        {
            throw new NaoEncontradoException($"Membro '{membroId}' não encontrado.");
        }

        if (!membro.Ativo)
        {
            throw new DomainException("Não é possível vincular um usuário a um membro desativado.");
        }
    }
}