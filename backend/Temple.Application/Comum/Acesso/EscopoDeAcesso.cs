using Temple.Domain.Entities;
using Temple.Domain.Enums;

namespace Temple.Application.Comum.Acesso;

public sealed class EscopoDeAcesso
{
    private EscopoDeAcesso(bool total, IReadOnlyList<Guid> igrejaIds, IReadOnlyList<GrupoMembro> grupos)
    {
        Total = total;
        IgrejaIds = igrejaIds;
        Grupos = grupos;
    }

    public bool Total { get; }

    public IReadOnlyList<Guid> IgrejaIds { get; }

    // Lista vazia: todos os grupos (e também os membros sem grupo).
    public IReadOnlyList<GrupoMembro> Grupos { get; }

    public static EscopoDeAcesso Completo()
    {
        return new EscopoDeAcesso(total: true, igrejaIds: [], grupos: []);
    }

    public static EscopoDeAcesso Nenhum()
    {
        return new EscopoDeAcesso(total: false, igrejaIds: [], grupos: []);
    }

    public static EscopoDeAcesso DasIgrejas(IReadOnlyList<Guid> igrejaIds)
    {
        return new EscopoDeAcesso(total: false, igrejaIds, grupos: []);
    }

    public static EscopoDeAcesso DosGrupos(Guid igrejaId, IReadOnlyList<GrupoMembro> grupos)
    {
        return new EscopoDeAcesso(total: false, igrejaIds: [igrejaId], grupos);
    }

    public bool AlcancaIgreja(Guid igrejaId)
    {
        return Total || IgrejaIds.Contains(igrejaId);
    }

    public bool AlcancaMembro(Membro membro)
    {
        if (!AlcancaIgreja(membro.IgrejaId))
        {
            return false;
        }

        if (Grupos.Count == 0)
        {
            return true;
        }

        return membro.Grupo is GrupoMembro grupo && Grupos.Contains(grupo);
    }
}