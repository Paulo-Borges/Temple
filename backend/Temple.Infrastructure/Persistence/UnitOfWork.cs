using Temple.Application.Comum.Persistencia;

namespace Temple.Infrastructure.Persistence;

internal sealed class UnitOfWork(TempleDbContext context) : IUnitOfWork
{
    public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
