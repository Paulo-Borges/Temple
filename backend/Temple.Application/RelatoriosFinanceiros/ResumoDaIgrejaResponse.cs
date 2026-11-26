namespace Temple.Application.RelatoriosFinanceiros;

public sealed record ResumoDaIgrejaResponse(
    Guid IgrejaId,
    string NomeDaIgreja,
    bool EhMatriz,
    decimal SaldoAnterior,
    decimal TotalEntradas,
    decimal TotalSaidas,
    decimal SaldoFinal);