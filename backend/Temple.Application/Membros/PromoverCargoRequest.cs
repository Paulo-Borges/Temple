using Temple.Domain.Enums;

namespace Temple.Application.Membros;

public sealed record PromoverCargoRequest(
    TipoCargo NovoCargo,
    DateOnly DataPromocao);
