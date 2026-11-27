namespace Temple.Application.Fechamentos;

public sealed record FechamentoResponse(
    Guid Id,
    Guid IgrejaId,
    int Ano,
    int Mes,
    decimal SaldoAnterior,
    decimal TotalEntradas,
    decimal TotalSaidas,
    decimal SaldoFinal,
    Guid FechadoPorId,
    string FechadoPorNome,
    DateTimeOffset FechadoEm,
    bool Reaberto,
    string? MotivoDaReabertura,
    Guid? ReabertoPorId,
    string? ReabertoPorNome,
    DateTimeOffset? ReabertoEm);