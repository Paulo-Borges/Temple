using Temple.Domain.Enums;

namespace Temple.Application.Lancamentos;

public sealed record CriarDespesaRequest(
    Guid IgrejaId,
    CategoriaDespesa Categoria,
    DateOnly Data,
    decimal Valor,
    FormaPagamento FormaPagamento,
    string Descricao);