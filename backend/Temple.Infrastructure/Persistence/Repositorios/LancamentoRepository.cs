using Microsoft.EntityFrameworkCore;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Lancamentos;
using Temple.Domain.Entities;
using Temple.Domain.Enums;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class LancamentoRepository(TempleDbContext context) : ILancamentoRepository
{
    public Task<Lancamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Lancamentos
            .FirstOrDefaultAsync(lancamento => lancamento.Id == id, cancellationToken);
    }


    public async Task<ResultadoPaginado<Lancamento>> ListarAsync(ListarLancamentosRequest filtro, EscopoDeAcesso escopo, CancellationToken cancellationToken)
    {
        IQueryable<Lancamento> consulta = context.Lancamentos.AsNoTracking();

        if (!escopo.Total)
        {
            IReadOnlyList<Guid> igrejaIds = escopo.IgrejaIds;
            consulta = consulta.Where(lancamento => igrejaIds.Contains(lancamento.IgrejaId));
        }

        if (filtro.IgrejaId is Guid igrejaId)
        {
            consulta = consulta.Where(lancamento => lancamento.IgrejaId == igrejaId);
        }

        if (filtro.De is DateOnly de)
        {
            consulta = consulta.Where(lancamento => lancamento.Data >= de);
        }

        if (filtro.Ate is DateOnly ate)
        {
            consulta = consulta.Where(lancamento => lancamento.Data <= ate);
        }

        if (filtro.Tipo is TipoLancamento tipo)
        {
            consulta = consulta.Where(lancamento => lancamento.Tipo == tipo);
        }

        if (filtro.MembroId is Guid membroId)
        {
            consulta = consulta.Where(lancamento => lancamento.MembroId == membroId);
        }

        if (!filtro.IncluirEstornados)
        {
            consulta = consulta.Where(lancamento => lancamento.EstornadoEm == null);
        }

        int totalItens = await consulta.CountAsync(cancellationToken);

        // Mais recentes primeiro; no mesmo dia, pela ordem de registro.
        List<Lancamento> itens = await consulta
            .OrderByDescending(lancamento => lancamento.Data)
            .ThenByDescending(lancamento => lancamento.RegistradoEm)
            .ThenBy(lancamento => lancamento.Id)
            .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
            .Take(filtro.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Lancamento>(itens, totalItens);
    }

    public async Task<DateOnly?> ObterDataDoPrimeiroLancamentoAsync(Guid igrejaId, CancellationToken cancellationToken)
    {
        List<DateOnly> datas = await context.Lancamentos
            .AsNoTracking()
            .Where(lancamento => lancamento.IgrejaId == igrejaId)
            .OrderBy(lancamento => lancamento.Data)
            .Select(lancamento => lancamento.Data)
            .Take(1)
            .ToListAsync(cancellationToken);

        if (datas.Count == 0)
        {
            return null;
        }

        return datas[0];
    }
    public void Adicionar(Lancamento lancamento)
    {
        context.Lancamentos.Add(lancamento);
    }
}