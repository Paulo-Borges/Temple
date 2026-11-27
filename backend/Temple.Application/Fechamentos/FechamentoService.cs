using FluentValidation;
using Temple.Application.Autenticacao;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Igrejas;
using Temple.Application.Lancamentos;
using Temple.Application.RelatoriosFinanceiros;
using Temple.Domain.Entities;

namespace Temple.Application.Fechamentos;

public sealed class FechamentoService(
    IFechamentoRepository fechamentoRepository,
    ILancamentoRepository lancamentoRepository,
    IRelatorioFinanceiroRepository relatorioRepository,
    IIgrejaRepository igrejaRepository,
    IUnitOfWork unitOfWork,
    ProvedorDeEscopo provedorDeEscopo,
    IServicoDeIdentidade servicoDeIdentidade,
    IUsuarioAtual usuarioAtual,
    TimeProvider timeProvider,
    IValidator<FecharMesRequest> fecharValidator,
    IValidator<ReabrirMesRequest> reabrirValidator)
{
    public async Task<FechamentoResponse> FecharAsync(FecharMesRequest request, CancellationToken cancellationToken)
    {
        await fecharValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        UsuarioAutenticado autor = await ObterAutorAsync(cancellationToken);

        FechamentoMensal? ultimoFechado = await fechamentoRepository.ObterUltimoAsync(igreja.Id, cancellationToken);
        DateOnly? dataDoPrimeiroLancamento = await lancamentoRepository.ObterDataDoPrimeiroLancamentoAsync(igreja.Id, cancellationToken);

        // A foto dos totais usa o mesmo cálculo do relatório mensal.
        DateOnly inicioDoMes = new(request.Ano, request.Mes, 1);
        DateOnly fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);
        IReadOnlyList<TotalDeLancamentos> totais = await relatorioRepository.SomarAsync([igreja.Id], inicioDoMes, fimDoMes, cancellationToken);

        FechamentoMensal fechamento = FechamentoMensal.Fechar(
            igreja,
            request.Ano,
            request.Mes,
            ultimoFechado,
            dataDoPrimeiroLancamento,
            CalculoDoSaldo.SaldoAntesDoMes(totais),
            CalculoDoSaldo.EntradasDoMes(totais),
            CalculoDoSaldo.SaidasDoMes(totais),
            autor.Id,
            autor.Nome,
            timeProvider.GetLocalNow());

        fechamentoRepository.Adicionar(fechamento);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return fechamento.ParaResponse();
    }

    public async Task<FechamentoResponse> ReabrirAsync(ReabrirMesRequest request, CancellationToken cancellationToken)
    {
        await reabrirValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        FechamentoMensal? fechamento = await fechamentoRepository.ObterDoMesAsync(igreja.Id, request.Ano, request.Mes, cancellationToken);

        if (fechamento is null)
        {
            throw new NaoEncontradoException($"O mês {request.Mes:D2}/{request.Ano} não está fechado.");
        }

        FechamentoMensal? ultimoFechado = await fechamentoRepository.ObterUltimoAsync(igreja.Id, cancellationToken);
        bool ehOUltimoFechado = ultimoFechado is not null && ultimoFechado.Id == fechamento.Id;
        UsuarioAutenticado autor = await ObterAutorAsync(cancellationToken);

        fechamento.Reabrir(ehOUltimoFechado, request.Motivo, autor.Id, autor.Nome, timeProvider.GetLocalNow());

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return fechamento.ParaResponse();
    }

    public async Task<IReadOnlyList<FechamentoResponse>> ListarAsync(Guid igrejaId, CancellationToken cancellationToken)
    {
        Igreja igreja = await ObterIgrejaAsync(igrejaId, cancellationToken);

        IReadOnlyList<FechamentoMensal> fechamentos = await fechamentoRepository.ListarAsync(igreja.Id, cancellationToken);

        return fechamentos
            .Select(fechamento => fechamento.ParaResponse())
            .ToList();
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