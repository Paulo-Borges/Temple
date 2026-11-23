using Temple.Application.Autenticacao;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Igrejas;
using Temple.Application.Membros;
using Temple.Domain.Entities;
using Temple.Domain.Enums;

namespace Temple.Application.Comum.Acesso;

public sealed class ProvedorDeEscopo(
    IUsuarioAtual usuarioAtual,
    IServicoDeIdentidade servicoDeIdentidade,
    IMembroRepository membroRepository,
    IIgrejaRepository igrejaRepository)
{
    private static readonly Perfil[] PerfisDaIgreja = [Perfil.Pastor, Perfil.Secretario, Perfil.Tesoureiro];

    private static readonly Dictionary<Perfil, GrupoMembro> GrupoDoLider = new()
    {
        [Perfil.LiderVaroes] = GrupoMembro.Varoes,
        [Perfil.LiderIrmas] = GrupoMembro.Irmas,
        [Perfil.LiderJovens] = GrupoMembro.Jovens,
        [Perfil.LiderCriancas] = GrupoMembro.Criancas
    };

    private EscopoDeAcesso? _escopo;

    public async Task<EscopoDeAcesso> ObterAsync(CancellationToken cancellationToken)
    {
        if (_escopo is EscopoDeAcesso escopoCalculado)
        {
            return escopoCalculado;
        }

        EscopoDeAcesso escopo = await CalcularAsync(cancellationToken);
        _escopo = escopo;

        return escopo;
    }

    private async Task<EscopoDeAcesso> CalcularAsync(CancellationToken cancellationToken)
    {
        UsuarioAutenticado? usuario = await servicoDeIdentidade.ObterPorIdAsync(usuarioAtual.Id, cancellationToken);

        if (usuario is null)
        {
            throw new NaoAutorizadoException("O usuário do token não existe mais.");
        }

        if (usuario.MembroId is not Guid membroId)
        {
            return usuario.Perfis.Contains(Perfil.Pastor) ? EscopoDeAcesso.Completo() : EscopoDeAcesso.Nenhum();
        }

        Membro? membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);

        if (membro is null || !membro.Ativo)
        {
            return EscopoDeAcesso.Nenhum();
        }

        if (usuario.Perfis.Any(perfil => PerfisDaIgreja.Contains(perfil)))
        {
            return await EscopoDaIgrejaComFiliaisAsync(membro.IgrejaId, cancellationToken);
        }

        if (usuario.Perfis.Contains(Perfil.Lider))
        {
            return EscopoDeAcesso.DasIgrejas([membro.IgrejaId]);
        }

        List<GrupoMembro> grupos = [];

        foreach (Perfil perfil in usuario.Perfis)
        {
            if (GrupoDoLider.TryGetValue(perfil, out GrupoMembro grupo))
            {
                grupos.Add(grupo);
            }
        }

        if (grupos.Count > 0)
        {
            return EscopoDeAcesso.DosGrupos(membro.IgrejaId, grupos);
        }

        return EscopoDeAcesso.Nenhum();
    }

    private async Task<EscopoDeAcesso> EscopoDaIgrejaComFiliaisAsync(Guid igrejaId, CancellationToken cancellationToken)
    {
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(igrejaId, cancellationToken);

        if (igreja is null)
        {
            return EscopoDeAcesso.Nenhum();
        }

        List<Guid> igrejaIds = [igreja.Id];

        if (igreja.EhMatriz)
        {
            igrejaIds.AddRange(igreja.Filiais.Select(filial => filial.Id));
        }

        return EscopoDeAcesso.DasIgrejas(igrejaIds);
    }
}