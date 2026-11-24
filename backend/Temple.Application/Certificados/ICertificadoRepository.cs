using Temple.Domain.Entities;

namespace Temple.Application.Certificados;

public interface ICertificadoRepository
{
    Task<CertificadoEmitido?> ObterPorNumeroAsync(int numero, CancellationToken cancellationToken);

    Task<bool> ExisteParaPromocaoAsync(Guid historicoCargoId, CancellationToken cancellationToken);

    void Adicionar(CertificadoEmitido certificado);
}