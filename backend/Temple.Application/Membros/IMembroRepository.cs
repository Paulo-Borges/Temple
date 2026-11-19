using Temple.Application.Comum.Persistencia;
using Temple.Domain.Entities;

namespace Temple.Application.Membros;

public interface IMembroRepository
{
    Task<Membro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ResultadoPaginado<Membro>> ListarAsync(ListarMembrosRequest filtro, CancellationToken cancellationToken);

    void Adicionar(Membro membro);
}
