namespace Temple.Application.Comum.Persistencia;

public sealed record ResultadoPaginado<T>(
    IReadOnlyList<T> Itens,
    int TotalItens);
