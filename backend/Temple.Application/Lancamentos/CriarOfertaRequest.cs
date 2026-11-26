using Temple.Domain.Enums;

namespace Temple.Application.Lancamentos;

// Sem MembroId, a oferta é anônima.
public sealed record CriarOfertaRequest(
    Guid IgrejaId,
    DateOnly Data,
    decimal Valor,
    FormaPagamento FormaPagamento,
    Guid? MembroId = null,
    string? Observacao = null);