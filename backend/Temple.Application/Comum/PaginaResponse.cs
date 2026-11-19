namespace Temple.Application.Comum;

public sealed record PaginaResponse<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas);
