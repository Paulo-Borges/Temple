using Temple.Domain.Enums;

namespace Temple.Application.Lancamentos;

// Todos os filtros são opcionais. Os estornados aparecem, a não ser que IncluirEstornados seja false.
public sealed record ListarLancamentosRequest(
    Guid? IgrejaId,
    DateOnly? De,
    DateOnly? Ate,
    TipoLancamento? Tipo,
    Guid? MembroId,
    bool IncluirEstornados = true,
    int Pagina = 1,
    int TamanhoPagina = 20);