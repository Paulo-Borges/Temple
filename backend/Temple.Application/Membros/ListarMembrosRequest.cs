using Temple.Domain.Enums;

namespace Temple.Application.Membros;

public sealed record ListarMembrosRequest(
    Guid? IgrejaId,
    string? Nome,
    TipoCargo? Cargo,
    bool? Ativo,
    int Pagina = 1,
    int TamanhoPagina = 20);
