using Microsoft.EntityFrameworkCore;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Membros;
using Temple.Domain.Entities;
using Temple.Domain.Enums;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class MembroRepository(TempleDbContext context) : IMembroRepository
{
    public Task<Membro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Membros
            .Include(membro => membro.HistoricoCargos)
            .FirstOrDefaultAsync(membro => membro.Id == id, cancellationToken);
    }

       public async Task<ResultadoPaginado<Membro>> ListarAsync(ListarMembrosRequest filtro, EscopoDeAcesso escopo, CancellationToken cancellationToken)
    {
        IQueryable<Membro> consulta = context.Membros.AsNoTracking();

                if (!escopo.Total)
        {
            IReadOnlyList<Guid> igrejaIds = escopo.IgrejaIds;
            consulta = consulta.Where(membro => igrejaIds.Contains(membro.IgrejaId));
        }

        if (escopo.Grupos.Count > 0)
        {
            List<GrupoMembro?> grupos = [];

            foreach (GrupoMembro grupo in escopo.Grupos)
            {
                grupos.Add(grupo);
            }

            consulta = consulta.Where(membro => grupos.Contains(membro.Grupo));
        }

        if (filtro.IgrejaId is Guid igrejaId)
        {
            consulta = consulta.Where(membro => membro.IgrejaId == igrejaId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            string nome = filtro.Nome.Trim();
            consulta = consulta.Where(membro => membro.Nome.Contains(nome));
        }

        if (filtro.Cargo is TipoCargo cargo)
        {
            consulta = consulta.Where(membro => membro.CargoAtual == cargo);
        }

        if (filtro.Ativo is bool ativo)
        {
            consulta = consulta.Where(membro => membro.Ativo == ativo);
        }

        int totalItens = await consulta.CountAsync(cancellationToken);

        List<Membro> itens = await consulta
            .OrderBy(membro => membro.Nome)
            .ThenBy(membro => membro.Matricula)
            .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
            .Take(filtro.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Membro>(itens, totalItens);
    }

    public void Adicionar(Membro membro)
    {
        context.Membros.Add(membro);
    }
}
