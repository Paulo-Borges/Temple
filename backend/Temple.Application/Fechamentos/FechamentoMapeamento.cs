using Temple.Domain.Entities;

namespace Temple.Application.Fechamentos;

public static class FechamentoMapeamento
{
    public static FechamentoResponse ParaResponse(this FechamentoMensal fechamento)
    {
        return new FechamentoResponse(
            fechamento.Id,
            fechamento.IgrejaId,
            fechamento.Ano,
            fechamento.Mes,
            fechamento.SaldoAnterior,
            fechamento.TotalEntradas,
            fechamento.TotalSaidas,
            fechamento.SaldoFinal,
            fechamento.FechadoPorId,
            fechamento.FechadoPorNome,
            fechamento.FechadoEm,
            fechamento.Reaberto,
            fechamento.MotivoDaReabertura,
            fechamento.ReabertoPorId,
            fechamento.ReabertoPorNome,
            fechamento.ReabertoEm);
    }
}