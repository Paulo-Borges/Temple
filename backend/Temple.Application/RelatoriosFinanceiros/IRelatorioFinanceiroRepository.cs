namespace Temple.Application.RelatoriosFinanceiros;

public interface IRelatorioFinanceiroRepository
{
    // Soma os lançamentos não estornados das igrejas, do início do histórico até o fim do mês.
    Task<IReadOnlyList<TotalDeLancamentos>> SomarAsync(IReadOnlyList<Guid> igrejaIds, DateOnly inicioDoMes, DateOnly fimDoMes, CancellationToken cancellationToken);
}