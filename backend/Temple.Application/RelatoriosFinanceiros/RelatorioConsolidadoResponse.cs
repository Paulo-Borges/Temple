namespace Temple.Application.RelatoriosFinanceiros;

// Os totais somam a matriz e todas as filiais; Igrejas traz o resumo de cada uma.
public sealed record RelatorioConsolidadoResponse(
    Guid MatrizId,
    string NomeDaMatriz,
    int Ano,
    int Mes,
    decimal SaldoAnterior,
    decimal TotalEntradas,
    decimal TotalSaidas,
    decimal SaldoFinal,
    IReadOnlyList<ResumoDaIgrejaResponse> Igrejas);