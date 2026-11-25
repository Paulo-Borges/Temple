using Microsoft.EntityFrameworkCore;
using Temple.Application.Apresentacoes;
using Temple.Application.Comum.Acesso;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class ApresentacaoRepository(TempleDbContext context) : IApresentacaoRepository
{
    public Task<ApresentacaoCrianca?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.ApresentacoesCriancas
            .FirstOrDefaultAsync(apresentacao => apresentacao.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ApresentacaoCrianca>> ListarAsync(Guid? igrejaId, EscopoDeAcesso escopo, CancellationToken cancellationToken)
    {
        IQueryable<ApresentacaoCrianca> consulta = context.ApresentacoesCriancas.AsNoTracking();

        if (!escopo.Total)
        {
            IReadOnlyList<Guid> igrejaIds = escopo.IgrejaIds;
            consulta = consulta.Where(apresentacao => igrejaIds.Contains(apresentacao.IgrejaId));
        }

        if (igrejaId is Guid id)
        {
            consulta = consulta.Where(apresentacao => apresentacao.IgrejaId == id);
        }

        return await consulta
            .OrderByDescending(apresentacao => apresentacao.DataApresentacao)
            .ThenBy(apresentacao => apresentacao.NomeDaCrianca)
            .ToListAsync(cancellationToken);
    }

    public void Adicionar(ApresentacaoCrianca apresentacao)
    {
        context.ApresentacoesCriancas.Add(apresentacao);
    }
}