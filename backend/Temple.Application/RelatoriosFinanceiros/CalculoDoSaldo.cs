using Temple.Domain.Entities;
using Temple.Domain.Enums;

namespace Temple.Application.RelatoriosFinanceiros;

// O cálculo do saldo num lugar só: usado pelos relatórios e pelo fechamento do mês.
internal static class CalculoDoSaldo
{
    // Entrada soma, saída subtrai: a mesma regra do ValorNoSaldo do lançamento.
    public static decimal SaldoAntesDoMes(IReadOnlyList<TotalDeLancamentos> totais)
    {
        return totais
            .Where(total => total.AntesDoMes)
            .Sum(total => Lancamento.NaturezaDo(total.Tipo) == NaturezaLancamento.Entrada ? total.Total : -total.Total);
    }

    public static decimal EntradasDoMes(IReadOnlyList<TotalDeLancamentos> totais)
    {
        return SomarDoMes(totais, NaturezaLancamento.Entrada);
    }

    public static decimal SaidasDoMes(IReadOnlyList<TotalDeLancamentos> totais)
    {
        return SomarDoMes(totais, NaturezaLancamento.Saida);
    }

    private static decimal SomarDoMes(IReadOnlyList<TotalDeLancamentos> totais, NaturezaLancamento natureza)
    {
        return totais
            .Where(total => !total.AntesDoMes && Lancamento.NaturezaDo(total.Tipo) == natureza)
            .Sum(total => total.Total);
    }
}