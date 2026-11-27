namespace Temple.Application.Fechamentos;

public sealed record FecharMesRequest(
    Guid IgrejaId,
    int Ano,
    int Mes);