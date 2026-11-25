using Temple.Application.Comum.Acesso;
using Temple.Domain.Entities;

namespace Temple.Application.Apresentacoes;

public interface IApresentacaoRepository
{
    Task<ApresentacaoCrianca?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ApresentacaoCrianca>> ListarAsync(Guid? igrejaId, EscopoDeAcesso escopo, CancellationToken cancellationToken);

    void Adicionar(ApresentacaoCrianca apresentacao);
}