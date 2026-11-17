using Microsoft.EntityFrameworkCore;
using Temple.Application.Igrejas;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class IgrejaRepository(TempleDbContext context) : IIgrejaRepository
{
    public Task<Igreja?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Igrejas.FirstOrDefaultAsync(igreja => igreja.Id == id, cancellationToken);
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
