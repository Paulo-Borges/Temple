using Microsoft.AspNetCore.Authorization;
using Temple.Domain.Enums;
using Temple.Infrastructure.Identidade;

namespace Temple.Api.Autorizacao;

internal static class PoliticasDeAutorizacao
{
    public static AuthorizationBuilder AdicionarPoliticasDoTemple(this AuthorizationBuilder autorizacao)
    {
        return autorizacao
            .AdicionarPolitica(Permissoes.VerIgrejas, Perfil.Pastor, Perfil.Secretario, Perfil.Tesoureiro)
            .AdicionarPolitica(Permissoes.GerenciarIgrejas, Perfil.Pastor)
            .AdicionarPolitica(Permissoes.VerMembros,
                Perfil.Pastor, Perfil.Secretario, Perfil.Tesoureiro,
                Perfil.Lider, Perfil.LiderVaroes, Perfil.LiderIrmas, Perfil.LiderJovens, Perfil.LiderCriancas)
            .AdicionarPolitica(Permissoes.GerenciarMembros, Perfil.Pastor, Perfil.Secretario)
            .AdicionarPolitica(Permissoes.PromoverMembros, Perfil.Pastor)
            .AdicionarPolitica(Permissoes.GerenciarUsuarios, Perfil.Pastor);
    }

    private static AuthorizationBuilder AdicionarPolitica(this AuthorizationBuilder autorizacao, string permissao, params Perfil[] perfis)
    {
        string[] papeis = perfis.Select(ConversorDePerfis.ParaNomeDoPapel).ToArray();

        return autorizacao.AddPolicy(permissao, politica => politica.RequireRole(papeis));
    }
}
