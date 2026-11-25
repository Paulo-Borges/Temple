using FluentValidation;
using Temple.Application.Certificados;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Membros;
using Temple.Domain.Entities;

namespace Temple.Application.Batismos;

public sealed class BatismoService(
    IBatismoRepository batismoRepository,
    IMembroRepository membroRepository,
    ICertificadoRepository certificadoRepository,
    IUnitOfWork unitOfWork,
    ProvedorDeEscopo provedorDeEscopo,
    TimeProvider timeProvider,
    IValidator<BatismoRequest> batismoValidator)
{
    public async Task<BatismoResponse> RegistrarAsync(Guid membroId, BatismoRequest request, CancellationToken cancellationToken)
    {
        await batismoValidator.ValidateAndThrowAsync(request, cancellationToken);

        Membro membro = await ObterMembroAsync(membroId, cancellationToken);

        if (await batismoRepository.ObterPorMembroAsync(membro.Id, cancellationToken) is not null)
        {
            throw new ConflitoException("O membro já tem batismo registrado. Para corrigir, use o PUT.");
        }

        Batismo batismo = Batismo.Registrar(membro, request.DataBatismo, request.Oficiante, request.Local, timeProvider.GetLocalNow());

        batismoRepository.Adicionar(batismo);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return batismo.ParaResponse();
    }

    public async Task<BatismoResponse> ObterAsync(Guid membroId, CancellationToken cancellationToken)
    {
        Membro membro = await ObterMembroAsync(membroId, cancellationToken);
        Batismo batismo = await ObterBatismoAsync(membro.Id, cancellationToken);

        return batismo.ParaResponse();
    }

    public async Task<BatismoResponse> CorrigirAsync(Guid membroId, BatismoRequest request, CancellationToken cancellationToken)
    {
        await batismoValidator.ValidateAndThrowAsync(request, cancellationToken);

        Membro membro = await ObterMembroAsync(membroId, cancellationToken);
        Batismo batismo = await ObterBatismoAsync(membro.Id, cancellationToken);
        bool temCertificadoEmitido = await certificadoRepository.ExisteParaBatismoAsync(batismo.Id, cancellationToken);

        batismo.Corrigir(membro, request.DataBatismo, request.Oficiante, request.Local, temCertificadoEmitido, timeProvider.GetLocalNow());

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return batismo.ParaResponse();
    }

    private async Task<Membro> ObterMembroAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro? membro = await membroRepository.ObterPorIdAsync(id, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (membro is null || !escopo.AlcancaMembro(membro))
        {
            throw new NaoEncontradoException($"Membro '{id}' não encontrado.");
        }

        return membro;
    }

    private async Task<Batismo> ObterBatismoAsync(Guid membroId, CancellationToken cancellationToken)
    {
        Batismo? batismo = await batismoRepository.ObterPorMembroAsync(membroId, cancellationToken);

        if (batismo is null)
        {
            throw new NaoEncontradoException("O membro não tem batismo registrado.");
        }

        return batismo;
    }
}