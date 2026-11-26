using Temple.Domain.Enums;

namespace Temple.Application.Lancamentos;

public sealed record CriarOutraEntradaRequest(
    Guid IgrejaId,
    DateOnly Data,
    decimal Valor,
    FormaPagamento FormaPagamento,
    string Descricao);