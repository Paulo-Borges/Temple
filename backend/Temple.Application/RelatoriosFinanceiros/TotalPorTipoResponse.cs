using Temple.Domain.Enums;

namespace Temple.Application.RelatoriosFinanceiros;

public sealed record TotalPorTipoResponse(
    TipoLancamento Tipo,
    decimal Total);