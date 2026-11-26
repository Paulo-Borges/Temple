namespace Temple.Application.RelatoriosFinanceiros;

// No consolidado, a IgrejaId precisa ser de uma matriz.
public sealed record RelatorioFinanceiroRequest(
    Guid IgrejaId,
    int Ano,
    int Mes);