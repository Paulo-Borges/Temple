using Temple.Domain.Enums;

namespace Temple.Domain.Entities;

public sealed class HistoricoCargo
{
    public Guid Id { get; private set; }
    public Guid MembroId { get; private set; }
    public TipoCargo CargoAnterior { get; private set; }
    public TipoCargo CargoNovo { get; private set; }
    public DateOnly DataPromocao { get; private set; }

    internal HistoricoCargo(Guid membroId, TipoCargo cargoAnterior, TipoCargo cargoNovo, DateOnly dataPromocao)
    {
        Id = Guid.NewGuid();
        MembroId = membroId;
        CargoAnterior = cargoAnterior;
        CargoNovo = cargoNovo;
        DataPromocao = dataPromocao;
    }
}
