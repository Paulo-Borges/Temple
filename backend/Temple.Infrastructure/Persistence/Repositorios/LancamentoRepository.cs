using Microsoft.EntityFrameworkCore;
using Temple.Application.Lancamentos;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class LancamentoRepository(TempleDbContext context) : ILancamentoRepository
{
    public Task<Lancamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Lancamentos
            .FirstOrDefaultAsync(lancamento => lancamento.Id == id, cancellationToken);
    }

    public void Adicionar(Lancamento lancamento)
    {
        context.Lancamentos.Add(lancamento);
    }
}