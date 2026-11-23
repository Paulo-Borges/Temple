using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Persistencia;
using Temple.Domain.Entities;

namespace Temple.Application.Membros;

public interface IMembroRepository
{
    Task<Membro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

     Task<ResultadoPaginado<Membro>> ListarAsync(ListarMembrosRequest filtro, EscopoDeAcesso escopo, CancellationToken cancellationToken);

    void Adicionar(Membro membro);
}
