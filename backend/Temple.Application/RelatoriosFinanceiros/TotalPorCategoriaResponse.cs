using Temple.Domain.Enums;

namespace Temple.Application.RelatoriosFinanceiros;

public sealed record TotalPorCategoriaResponse(
    CategoriaDespesa Categoria,
    decimal Total);