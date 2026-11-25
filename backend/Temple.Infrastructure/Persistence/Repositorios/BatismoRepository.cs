using Microsoft.EntityFrameworkCore;
using Temple.Application.Batismos;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class BatismoRepository(TempleDbContext context) : IBatismoRepository
{
    public Task<Batismo?> ObterPorMembroAsync(Guid membroId, CancellationToken cancellationToken)
    {
        return context.Batismos
            .FirstOrDefaultAsync(batismo => batismo.MembroId == membroId, cancellationToken);
    }

    public void Adicionar(Batismo batismo)
    {
        context.Batismos.Add(batismo);
    }
}