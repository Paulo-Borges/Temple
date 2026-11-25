using FluentValidation;
using Temple.Application.Certificados;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Igrejas;
using Temple.Domain.Entities;

namespace Temple.Application.Apresentacoes;

public sealed class ApresentacaoService(
    IApresentacaoRepository apresentacaoRepository,
    IIgrejaRepository igrejaRepository,
    ICertificadoRepository certificadoRepository,
    IUnitOfWork unitOfWork,
    ProvedorDeEscopo provedorDeEscopo,
    TimeProvider timeProvider,
    IValidator<CriarApresentacaoRequest> criarValidator,
    IValidator<AtualizarApresentacaoRequest> atualizarValidator)
{
    public async Task<ApresentacaoResponse> CriarAsync(CriarApresentacaoRequest request, CancellationToken cancellationToken)
    {
        await criarValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(request.IgrejaId, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (igreja is null || !escopo.AlcancaIgreja(igreja.Id))
        {
            throw new NaoEncontradoException($"Igreja '{request.IgrejaId}' não encontrada.");
        }

        ApresentacaoCrianca apresentacao = ApresentacaoCrianca.Registrar(
            igreja,
            request.NomeDaCrianca,
            request.DataNascimentoDaCrianca,
            request.NomeDoPai,
            request.NomeDaMae,
            request.DataApresentacao,
            request.Oficiante,
            timeProvider.GetLocalNow());

        apresentacaoRepository.Adicionar(apresentacao);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return apresentacao.ParaResponse();
    }

    public async Task<ApresentacaoResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        ApresentacaoCrianca apresentacao = await ObterEntidadeAsync(id, cancellationToken);

        return apresentacao.ParaResponse();
    }

    public async Task<IReadOnlyList<ApresentacaoResponse>> ListarAsync(Guid? igrejaId, CancellationToken cancellationToken)
    {
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        IReadOnlyList<ApresentacaoCrianca> apresentacoes = await apresentacaoRepository.ListarAsync(igrejaId, escopo, cancellationToken);

        return apresentacoes
            .Select(apresentacao => apresentacao.ParaResponse())
            .ToList();
    }

    public async Task<ApresentacaoResponse> AtualizarAsync(Guid id, AtualizarApresentacaoRequest request, CancellationToken cancellationToken)
    {
        await atualizarValidator.ValidateAndThrowAsync(request, cancellationToken);

        ApresentacaoCrianca apresentacao = await ObterEntidadeAsync(id, cancellationToken);
        bool temCertificadoEmitido = await certificadoRepository.ExisteParaApresentacaoAsync(apresentacao.Id, cancellationToken);

        apresentacao.Corrigir(
            request.NomeDaCrianca,
            request.DataNascimentoDaCrianca,
            request.NomeDoPai,
            request.NomeDaMae,
            request.DataApresentacao,
            request.Oficiante,
            temCertificadoEmitido,
            timeProvider.GetLocalNow());

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return apresentacao.ParaResponse();
    }

    private async Task<ApresentacaoCrianca> ObterEntidadeAsync(Guid id, CancellationToken cancellationToken)
    {
        ApresentacaoCrianca? apresentacao = await apresentacaoRepository.ObterPorIdAsync(id, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (apresentacao is null || !escopo.AlcancaIgreja(apresentacao.IgrejaId))
        {
            throw new NaoEncontradoException($"Apresentação '{id}' não encontrada.");
        }

        return apresentacao;
    }
}