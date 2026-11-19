using Temple.Domain.Entities;

namespace Temple.Application.Igrejas;

public interface IIgrejaRepository
{
    Task<Igreja?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Igreja>> ListarAsync(bool? ativa, CancellationToken cancellationToken);

    Task<bool> ExisteComCnpjAsync(string cnpj, CancellationToken cancellationToken);

    void Adicionar(Igreja igreja);
}
