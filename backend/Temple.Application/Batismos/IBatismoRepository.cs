using Temple.Domain.Entities;

namespace Temple.Application.Batismos;

public interface IBatismoRepository
{
    Task<Batismo?> ObterPorMembroAsync(Guid membroId, CancellationToken cancellationToken);

    void Adicionar(Batismo batismo);
}