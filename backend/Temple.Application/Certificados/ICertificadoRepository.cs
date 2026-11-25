using Temple.Domain.Entities;

namespace Temple.Application.Certificados;

public interface ICertificadoRepository
{
    Task<CertificadoEmitido?> ObterPorNumeroAsync(int numero, CancellationToken cancellationToken);

    Task<bool> ExisteParaPromocaoAsync(Guid historicoCargoId, CancellationToken cancellationToken);

        Task<bool> ExisteParaBatismoAsync(Guid batismoId, CancellationToken cancellationToken);

    Task<bool> ExisteParaApresentacaoAsync(Guid apresentacaoCriancaId, CancellationToken cancellationToken);

    void Adicionar(CertificadoEmitido certificado);
}