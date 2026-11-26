using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Persistencia;
using Temple.Domain.Entities;

namespace Temple.Application.Lancamentos;

public interface ILancamentoRepository
{
    Task<Lancamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ResultadoPaginado<Lancamento>> ListarAsync(ListarLancamentosRequest filtro, EscopoDeAcesso escopo, CancellationToken cancellationToken);

    void Adicionar(Lancamento lancamento);
}