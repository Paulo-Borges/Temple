namespace Temple.Application.RelatoriosFinanceiros;

// SaldoFinal = SaldoAnterior + TotalEntradas - TotalSaidas.
public sealed record RelatorioMensalResponse(
    Guid IgrejaId,
    string NomeDaIgreja,
    int Ano,
    int Mes,
    decimal SaldoAnterior,
    decimal TotalEntradas,
    decimal TotalSaidas,
    decimal SaldoFinal,
    IReadOnlyList<TotalPorTipoResponse> EntradasPorTipo,
    IReadOnlyList<TotalPorCategoriaResponse> SaidasPorCategoria);