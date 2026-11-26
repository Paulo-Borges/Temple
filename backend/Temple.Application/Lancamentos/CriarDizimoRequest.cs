using Temple.Domain.Enums;

namespace Temple.Application.Lancamentos;

public sealed record CriarDizimoRequest(
    Guid IgrejaId,
    Guid MembroId,
    DateOnly Data,
    decimal Valor,
    FormaPagamento FormaPagamento,
    string? Observacao = null);