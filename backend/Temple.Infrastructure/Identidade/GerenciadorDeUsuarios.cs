using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Usuarios;
using Temple.Domain.Enums;
using Temple.Infrastructure.Persistence;

namespace Temple.Infrastructure.Identidade;

internal sealed class GerenciadorDeUsuarios(UserManager<Usuario> userManager, TempleDbContext context) : IGerenciadorDeUsuarios
{
    public async Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        Usuario usuario = new(request.Nome, request.Email);
         usuario.VincularMembro(request.MembroId);

        await using IDbContextTransaction transacao = await context.Database.BeginTransactionAsync(cancellationToken);

        GarantirSucesso(await userManager.CreateAsync(usuario, request.Senha));
        GarantirSucesso(await userManager.AddToRolesAsync(usuario, request.Perfis.Select(ConversorDePerfis.ParaNomeDoPapel)));

        await transacao.CommitAsync(cancellationToken);

        return await ParaResponseAsync(usuario);
    }

    public async Task<UsuarioResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Usuario? usuario = await userManager.FindByIdAsync(id.ToString());

        if (usuario is null)
        {
            return null;
        }

        return await ParaResponseAsync(usuario);
    }

    public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken cancellationToken)
    {
        List<Usuario> usuarios = await userManager.Users
            .OrderBy(usuario => usuario.Nome)
            .ThenBy(usuario => usuario.Email)
            .ToListAsync(cancellationToken);

        List<UsuarioResponse> respostas = [];

        foreach (Usuario usuario in usuarios)
        {
            respostas.Add(await ParaResponseAsync(usuario));
        }

        return respostas;
    }

    public async Task<int> ContarComPerfilAsync(Perfil perfil, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IList<Usuario> usuarios = await userManager.GetUsersInRoleAsync(ConversorDePerfis.ParaNomeDoPapel(perfil));

        return usuarios.Count;
    }

    public async Task<UsuarioResponse> AlterarPerfisAsync(Guid id, IReadOnlyList<Perfil> perfis, CancellationToken cancellationToken)
    {
        Usuario? usuario = await userManager.FindByIdAsync(id.ToString());

        if (usuario is null)
        {
            throw new InvalidOperationException($"O usuário '{id}' deveria existir.");
        }

        IList<string> papeisAtuais = await userManager.GetRolesAsync(usuario);
        List<string> papeisNovos = perfis.Select(ConversorDePerfis.ParaNomeDoPapel).ToList();

        await using IDbContextTransaction transacao = await context.Database.BeginTransactionAsync(cancellationToken);

        GarantirSucesso(await userManager.RemoveFromRolesAsync(usuario, papeisAtuais.Except(papeisNovos)));
        GarantirSucesso(await userManager.AddToRolesAsync(usuario, papeisNovos.Except(papeisAtuais)));

        await transacao.CommitAsync(cancellationToken);

        return await ParaResponseAsync(usuario);
    }

        public async Task<Guid?> ObterIdDoUsuarioDoMembroAsync(Guid membroId, CancellationToken cancellationToken)
    {
        Usuario? usuario = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.MembroId == membroId, cancellationToken);

        if (usuario is null)
        {
            return null;
        }

        return usuario.Id;
    }

    public async Task<UsuarioResponse> VincularMembroAsync(Guid id, Guid membroId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Usuario? usuario = await userManager.FindByIdAsync(id.ToString());

        if (usuario is null)
        {
            throw new InvalidOperationException($"O usuário '{id}' deveria existir.");
        }

        usuario.VincularMembro(membroId);

        GarantirSucesso(await userManager.UpdateAsync(usuario));

        return await ParaResponseAsync(usuario);
    }


    private async Task<UsuarioResponse> ParaResponseAsync(Usuario usuario)
    {
        if (usuario.Email is not string email)
        {
            throw new InvalidOperationException($"O usuário '{usuario.Id}' está sem e-mail.");
        }

        IList<string> papeis = await userManager.GetRolesAsync(usuario);

                return new UsuarioResponse(usuario.Id, usuario.Nome, email, usuario.MembroId, ConversorDePerfis.ParaPerfis(papeis));

    }

    private static void GarantirSucesso(IdentityResult resultado)
    {
        if (resultado.Succeeded)
        {
            return;
        }

        if (resultado.Errors.Any(erro => erro.Code.StartsWith("Duplicate", StringComparison.Ordinal)))
        {
            throw new ConflitoException("Já existe um usuário com este e-mail.");
        }

        List<ValidationFailure> falhas = [];

        foreach (IdentityError erro in resultado.Errors)
        {
            if (erro.Code.StartsWith("Password", StringComparison.Ordinal))
            {
                falhas.Add(new ValidationFailure(nameof(CriarUsuarioRequest.Senha), erro.Description));
            }
            else if (erro.Code is "InvalidEmail" or "InvalidUserName")
            {
                falhas.Add(new ValidationFailure(nameof(CriarUsuarioRequest.Email), erro.Description));
            }
            else
            {
                throw new InvalidOperationException($"Erro inesperado do Identity: {erro.Code} - {erro.Description}");
            }
        }

        throw new ValidationException(falhas);
    }
}
