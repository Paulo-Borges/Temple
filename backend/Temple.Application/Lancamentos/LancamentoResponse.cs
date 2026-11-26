using Temple.Domain.Enums;

namespace Temple.Application.Lancamentos;

public sealed record LancamentoResponse(
    Guid Id,
    Guid IgrejaId,
    TipoLancamento Tipo,
    NaturezaLancamento Natureza,
    Guid? MembroId,
    CategoriaDespesa? Categoria,
    DateOnly Data,
    decimal Valor,
    FormaPagamento FormaPagamento,
    string? Descricao,
    Guid RegistradoPorId,
    string RegistradoPorNome,
    DateTimeOffset RegistradoEm,
    bool Estornado,
    string? MotivoDoEstorno,
    Guid? EstornadoPorId,
    string? EstornadoPorNome,
    DateTimeOffset? EstornadoEm);