using Microsoft.EntityFrameworkCore;
using Temple.Application.Comum.Acesso;
using Temple.Application.Igrejas;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class IgrejaRepository(TempleDbContext context) : IIgrejaRepository
{
    public Task<Igreja?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Igrejas
            .Include(igreja => igreja.IgrejaMatriz)
            .Include(igreja => igreja.Filiais)
            .FirstOrDefaultAsync(igreja => igreja.Id == id, cancellationToken);
    }

       public async Task<IReadOnlyList<Igreja>> ListarAsync(bool? ativa, EscopoDeAcesso escopo, CancellationToken cancellationToken)
    {
        IQueryable<Igreja> consulta = context.Igrejas.AsNoTracking();

        if (!escopo.Total)
        {
            IReadOnlyList<Guid> igrejaIds = escopo.IgrejaIds;
            consulta = consulta.Where(igreja => igrejaIds.Contains(igreja.Id));
        }

        if (ativa is bool valor)
        {
            consulta = consulta.Where(igreja => igreja.Ativa == valor);
        }

        return await consulta
            .OrderBy(igreja => igreja.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExisteComCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        return context.Igrejas.AnyAsync(igreja => igreja.Cnpj == cnpj, cancellationToken);
    }

    public void Adicionar(Igreja igreja)
    {
        context.Igrejas.Add(igreja);
    }
}
