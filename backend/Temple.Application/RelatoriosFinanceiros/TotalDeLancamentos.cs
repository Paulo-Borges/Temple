using Temple.Domain.Enums;

namespace Temple.Application.RelatoriosFinanceiros;

// Uma linha somada pelo banco: o total de uma igreja, de um tipo e de uma categoria, antes do mês ou dentro dele.
public sealed record TotalDeLancamentos(
    Guid IgrejaId,
    TipoLancamento Tipo,
    CategoriaDespesa? Categoria,
    bool AntesDoMes,
    decimal Total);