using Microsoft.EntityFrameworkCore;
using Temple.Application.Certificados;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Repositorios;

internal sealed class CertificadoRepository(TempleDbContext context) : ICertificadoRepository
{
    public Task<CertificadoEmitido?> ObterPorNumeroAsync(int numero, CancellationToken cancellationToken)
    {
        return context.CertificadosEmitidos
            .AsNoTracking()
            .FirstOrDefaultAsync(certificado => certificado.Numero == numero, cancellationToken);
    }

    public Task<bool> ExisteParaPromocaoAsync(Guid historicoCargoId, CancellationToken cancellationToken)
    {
        return context.CertificadosEmitidos
            .AnyAsync(certificado => certificado.HistoricoCargoId == historicoCargoId, cancellationToken);
    }

    public void Adicionar(CertificadoEmitido certificado)
    {
        context.CertificadosEmitidos.Add(certificado);
    }
}