using FluentValidation;
using Temple.Application.Autenticacao;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Igrejas;
using Temple.Application.Membros;
using Temple.Domain.Entities;

namespace Temple.Application.Lancamentos;

public sealed class LancamentoService(
    ILancamentoRepository lancamentoRepository,
    IIgrejaRepository igrejaRepository,
    IMembroRepository membroRepository,
    IUnitOfWork unitOfWork,
    ProvedorDeEscopo provedorDeEscopo,
    IServicoDeIdentidade servicoDeIdentidade,
    IUsuarioAtual usuarioAtual,
    TimeProvider timeProvider,
    IValidator<CriarDizimoRequest> dizimoValidator,
    IValidator<CriarOfertaRequest> ofertaValidator,
    IValidator<CriarOutraEntradaRequest> outraEntradaValidator,
    IValidator<CriarDespesaRequest> despesaValidator,
    IValidator<EstornarLancamentoRequest> estornoValidator)
{
    public async Task<LancamentoResponse> RegistrarDizimoAsync(CriarDizimoRequest request, CancellationToken cancellationToken)
    {
        await dizimoValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        Membro membro = await ObterMembroNoEscopoAsync(request.MembroId, cancellationToken);
        UsuarioAutenticado autor = await ObterAutorAsync(cancellationToken);

        Lancamento lancamento = Lancamento.Dizimo(
            igreja,
            membro,
            request.Data,
            request.Valor,
            request.FormaPagamento,
            request.Observacao,
            autor.Id,
            autor.Nome,
            timeProvider.GetLocalNow());

        return await AdicionarAsync(lancamento, cancellationToken);
    }

    public async Task<LancamentoResponse> RegistrarOfertaAsync(CriarOfertaRequest request, CancellationToken cancellationToken)
    {
        await ofertaValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        UsuarioAutenticado autor = await ObterAutorAsync(cancellationToken);

        // A oferta identificada aceita membro de qualquer igreja: basta existir.
        Membro? membro = null;

        if (request.MembroId is Guid membroId)
        {
            membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);

            if (membro is null)
            {
                throw new NaoEncontradoException($"Membro '{membroId}' não encontrado.");
            }
        }

        Lancamento lancamento = Lancamento.Oferta(
            igreja,
            membro,
            request.Data,
            request.Valor,
            request.FormaPagamento,
            request.Observacao,
            autor.Id,
            autor.Nome,
            timeProvider.GetLocalNow());

        return await AdicionarAsync(lancamento, cancellationToken);
    }

    public async Task<LancamentoResponse> RegistrarOutraEntradaAsync(CriarOutraEntradaRequest request, CancellationToken cancellationToken)
    {
        await outraEntradaValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        UsuarioAutenticado autor = await ObterAutorAsync(cancellationToken);

        Lancamento lancamento = Lancamento.OutraEntrada(
            igreja,
            request.Data,
            request.Valor,
            request.FormaPagamento,
            request.Descricao,
            autor.Id,
            autor.Nome,
            timeProvider.GetLocalNow());

        return await AdicionarAsync(lancamento, cancellationToken);
    }

    public async Task<LancamentoResponse> RegistrarDespesaAsync(CriarDespesaRequest request, CancellationToken cancellationToken)
    {
        await despesaValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        UsuarioAutenticado autor = await ObterAutorAsync(cancellationToken);

        Lancamento lancamento = Lancamento.Despesa(
            igreja,
            request.Categoria,
            request.Data,
            request.Valor,
            request.FormaPagamento,
            request.Descricao,
            autor.Id,
            autor.Nome,
            timeProvider.GetLocalNow());

        return await AdicionarAsync(lancamento, cancellationToken);
    }

    public async Task<LancamentoResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Lancamento lancamento = await ObterEntidadeAsync(id, cancellationToken);

        return lancamento.ParaResponse();
    }

    public async Task<LancamentoResponse> EstornarAsync(Guid id, EstornarLancamentoRequest request, CancellationToken cancellationToken)
    {
        await estornoValidator.ValidateAndThrowAsync(request, cancellationToken);

        Lancamento lancamento = await ObterEntidadeAsync(id, cancellationToken);
        UsuarioAutenticado autor = await ObterAutorAsync(cancellationToken);

        lancamento.Estornar(request.Motivo, autor.Id, autor.Nome, timeProvider.GetLocalNow());

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return lancamento.ParaResponse();
    }

    private async Task<LancamentoResponse> AdicionarAsync(Lancamento lancamento, CancellationToken cancellationToken)
    {
        lancamentoRepository.Adicionar(lancamento);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return lancamento.ParaResponse();
    }

    private async Task<Lancamento> ObterEntidadeAsync(Guid id, CancellationToken cancellationToken)
    {
        Lancamento? lancamento = await lancamentoRepository.ObterPorIdAsync(id, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (lancamento is null || !escopo.AlcancaIgreja(lancamento.IgrejaId))
        {
            throw new NaoEncontradoException($"Lançamento '{id}' não encontrado.");
        }

        return lancamento;
    }

    private async Task<Igreja> ObterIgrejaAsync(Guid id, CancellationToken cancellationToken)
    {
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(id, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (igreja is null || !escopo.AlcancaIgreja(igreja.Id))
        {
            throw new NaoEncontradoException($"Igreja '{id}' não encontrada.");
        }

        return igreja;
    }

    private async Task<Membro> ObterMembroNoEscopoAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro? membro = await membroRepository.ObterPorIdAsync(id, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (membro is null || !escopo.AlcancaMembro(membro))
        {
            throw new NaoEncontradoException($"Membro '{id}' não encontrado.");
        }

        return membro;
    }

    private async Task<UsuarioAutenticado> ObterAutorAsync(CancellationToken cancellationToken)
    {
        UsuarioAutenticado? autor = await servicoDeIdentidade.ObterPorIdAsync(usuarioAtual.Id, cancellationToken);

        if (autor is null)
        {
            throw new NaoAutorizadoException("O usuário do token não existe mais.");
        }

        return autor;
    }
}