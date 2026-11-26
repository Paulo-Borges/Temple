using Temple.Domain.Entities;

namespace Temple.Application.Lancamentos;

public interface ILancamentoRepository
{
    Task<Lancamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    void Adicionar(Lancamento lancamento);
}