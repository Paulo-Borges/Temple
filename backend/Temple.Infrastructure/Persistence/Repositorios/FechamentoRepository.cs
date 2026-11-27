using Microsoft.EntityFrameworkCore;
using Temple.Application.Fechamentos;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class FechamentoRepository(TempleDbContext context) : IFechamentoRepository
{
    public Task<FechamentoMensal?> ObterUltimoAsync(Guid igrejaId, CancellationToken cancellationToken)
    {
        return context.FechamentosMensais
            .Where(fechamento => fechamento.IgrejaId == igrejaId && fechamento.ReabertoEm == null)
            .OrderByDescending(fechamento => fechamento.Ano)
            .ThenByDescending(fechamento => fechamento.Mes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<FechamentoMensal?> ObterDoMesAsync(Guid igrejaId, int ano, int mes, CancellationToken cancellationToken)
    {
        return context.FechamentosMensais
            .FirstOrDefaultAsync(
                fechamento => fechamento.IgrejaId == igrejaId
                    && fechamento.Ano == ano
                    && fechamento.Mes == mes
                    && fechamento.ReabertoEm == null,
                cancellationToken);
    }

    public async Task<IReadOnlyList<FechamentoMensal>> ListarAsync(Guid igrejaId, CancellationToken cancellationToken)
    {
        return await context.FechamentosMensais
            .AsNoTracking()
            .Where(fechamento => fechamento.IgrejaId == igrejaId)
            .OrderByDescending(fechamento => fechamento.Ano)
            .ThenByDescending(fechamento => fechamento.Mes)
            .ThenByDescending(fechamento => fechamento.FechadoEm)
            .ToListAsync(cancellationToken);
    }

    public void Adicionar(FechamentoMensal fechamento)
    {
        context.FechamentosMensais.Add(fechamento);
    }
}