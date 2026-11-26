using Microsoft.EntityFrameworkCore;
using Temple.Application.RelatoriosFinanceiros;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class RelatorioFinanceiroRepository(TempleDbContext context) : IRelatorioFinanceiroRepository
{
    public async Task<IReadOnlyList<TotalDeLancamentos>> SomarAsync(IReadOnlyList<Guid> igrejaIds, DateOnly inicioDoMes, DateOnly fimDoMes, CancellationToken cancellationToken)
    {
        // A soma é feita pelo banco (GROUP BY + SUM): só as linhas de total voltam, nunca os lançamentos.
        return await context.Lancamentos
            .AsNoTracking()
            .Where(lancamento => igrejaIds.Contains(lancamento.IgrejaId)
                && lancamento.Data <= fimDoMes
                && lancamento.EstornadoEm == null)
            .GroupBy(lancamento => new
            {
                lancamento.IgrejaId,
                lancamento.Tipo,
                lancamento.Categoria,
                AntesDoMes = lancamento.Data < inicioDoMes
            })
            .Select(grupo => new TotalDeLancamentos(
                grupo.Key.IgrejaId,
                grupo.Key.Tipo,
                grupo.Key.Categoria,
                grupo.Key.AntesDoMes,
                grupo.Sum(lancamento => lancamento.Valor)))
            .ToListAsync(cancellationToken);
    }
}