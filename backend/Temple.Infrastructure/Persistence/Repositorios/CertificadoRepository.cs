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

        public Task<bool> ExisteParaBatismoAsync(Guid batismoId, CancellationToken cancellationToken)
    {
        return context.CertificadosEmitidos
            .AnyAsync(certificado => certificado.BatismoId == batismoId, cancellationToken);
    }

    public Task<bool> ExisteParaApresentacaoAsync(Guid apresentacaoCriancaId, CancellationToken cancellationToken)
    {
        return context.CertificadosEmitidos
            .AnyAsync(certificado => certificado.ApresentacaoCriancaId == apresentacaoCriancaId, cancellationToken);
    }

    public void Adicionar(CertificadoEmitido certificado)
    {
        context.CertificadosEmitidos.Add(certificado);
    }
}