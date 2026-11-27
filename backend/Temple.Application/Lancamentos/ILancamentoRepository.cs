using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Persistencia;
using Temple.Domain.Entities;

namespace Temple.Application.Lancamentos;

public interface ILancamentoRepository
{
    Task<Lancamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ResultadoPaginado<Lancamento>> ListarAsync(ListarLancamentosRequest filtro, EscopoDeAcesso escopo, CancellationToken cancellationToken);

        // A data do lançamento mais antigo da igreja (estornado ou não), ou nulo se ela não tiver lançamentos.
    Task<DateOnly?> ObterDataDoPrimeiroLancamentoAsync(Guid igrejaId, CancellationToken cancellationToken);
    void Adicionar(Lancamento lancamento);
}