namespace Temple.Application.Fechamentos;

public sealed record ReabrirMesRequest(
    Guid IgrejaId,
    int Ano,
    int Mes,
    string Motivo);