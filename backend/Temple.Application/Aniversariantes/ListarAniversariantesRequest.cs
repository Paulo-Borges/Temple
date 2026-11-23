namespace Temple.Application.Aniversariantes;

public sealed record ListarAniversariantesRequest(
    int? Mes,
    Guid? IgrejaId);