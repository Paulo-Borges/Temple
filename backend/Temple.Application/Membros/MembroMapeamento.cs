using Temple.Application.Comum;
using Temple.Domain.Entities;

namespace Temple.Application.Membros;

public static class MembroMapeamento
{
    public static MembroResponse ParaResponse(this Membro membro)
    {
        return new MembroResponse(
            membro.Id,
            membro.Matricula,
            membro.Nome,
            membro.DataNascimento,
            membro.Endereco.ParaDto(),
            membro.IgrejaId,
            membro.CargoAtual,
            membro.Grupo,
            membro.Ativo);
    }

    public static HistoricoCargoResponse ParaResponse(this HistoricoCargo historico)
    {
        return new HistoricoCargoResponse(
            historico.Id,
            historico.CargoAnterior,
            historico.CargoNovo,
            historico.DataPromocao);
    }

    public static Membro ParaMembro(this CriarMembroRequest request, Igreja igreja)
    {
        ArgumentNullException.ThrowIfNull(igreja);

        return new Membro(
            request.Nome,
            request.DataNascimento,
            request.Endereco.ParaEndereco(),
            igreja,
            request.CargoAtual,
            request.Grupo);
    }
}
