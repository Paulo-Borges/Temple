using Temple.Domain.Enums;

namespace Temple.Application.Membros;

public sealed record HistoricoCargoResponse(
    Guid Id,
    TipoCargo CargoAnterior,
    TipoCargo CargoNovo,
    DateOnly DataPromocao);
