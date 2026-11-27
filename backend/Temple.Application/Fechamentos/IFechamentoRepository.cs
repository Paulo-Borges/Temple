using Temple.Domain.Entities;

namespace Temple.Application.Fechamentos;

public interface IFechamentoRepository
{
    // O fechamento ativo (não reaberto) mais recente da igreja.
    Task<FechamentoMensal?> ObterUltimoAsync(Guid igrejaId, CancellationToken cancellationToken);

    // O fechamento ativo do mês, se houver.
    Task<FechamentoMensal?> ObterDoMesAsync(Guid igrejaId, int ano, int mes, CancellationToken cancellationToken);

    // Todos os fechamentos da igreja, inclusive os reabertos, do mais recente para o mais antigo.
    Task<IReadOnlyList<FechamentoMensal>> ListarAsync(Guid igrejaId, CancellationToken cancellationToken);

    void Adicionar(FechamentoMensal fechamento);
}