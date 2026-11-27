using FluentValidation;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Igrejas;
using Temple.Domain.Entities;
using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Application.RelatoriosFinanceiros;

public sealed class RelatorioFinanceiroService(
    IRelatorioFinanceiroRepository relatorioRepository,
    IIgrejaRepository igrejaRepository,
    ProvedorDeEscopo provedorDeEscopo,
    IValidator<RelatorioFinanceiroRequest> validator)
{
    // Os tipos de entrada vêm da regra do Domain, e não de uma lista escrita aqui.
    private static readonly TipoLancamento[] TiposDeEntrada = Enum.GetValues<TipoLancamento>()
        .Where(tipo => Lancamento.NaturezaDo(tipo) == NaturezaLancamento.Entrada)
        .ToArray();

    public async Task<RelatorioMensalResponse> ObterMensalAsync(RelatorioFinanceiroRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        DateOnly inicioDoMes = new(request.Ano, request.Mes, 1);
        DateOnly fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

        IReadOnlyList<TotalDeLancamentos> totais = await relatorioRepository.SomarAsync([igreja.Id], inicioDoMes, fimDoMes, cancellationToken);

        List<TotalDeLancamentos> doMes = totais.Where(total => !total.AntesDoMes).ToList();

        List<TotalPorTipoResponse> entradasPorTipo = TiposDeEntrada
            .Select(tipo => new TotalPorTipoResponse(tipo, doMes.Where(total => total.Tipo == tipo).Sum(total => total.Total)))
            .ToList();

        List<TotalPorCategoriaResponse> saidasPorCategoria = Enum.GetValues<CategoriaDespesa>()
            .Select(categoria => new TotalPorCategoriaResponse(categoria, doMes.Where(total => total.Categoria == categoria).Sum(total => total.Total)))
            .ToList();

        decimal saldoAnterior = CalculoDoSaldo.SaldoAntesDoMes(totais);
        decimal totalEntradas = entradasPorTipo.Sum(entrada => entrada.Total);
        decimal totalSaidas = saidasPorCategoria.Sum(saida => saida.Total);

        return new RelatorioMensalResponse(
            igreja.Id,
            igreja.Nome,
            request.Ano,
            request.Mes,
            saldoAnterior,
            totalEntradas,
            totalSaidas,
            saldoAnterior + totalEntradas - totalSaidas,
            entradasPorTipo,
            saidasPorCategoria);
    }

    public async Task<RelatorioConsolidadoResponse> ObterConsolidadoAsync(RelatorioFinanceiroRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja matriz = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);

        if (!matriz.EhMatriz)
        {
            throw new DomainException("O relatório consolidado é da matriz com as filiais. Informe uma igreja matriz.");
        }

        // A matriz primeiro, depois as filiais (ativas ou não) em ordem de nome.
        List<Igreja> igrejas = [matriz, .. matriz.Filiais.OrderBy(filial => filial.Nome)];
        List<Guid> igrejaIds = igrejas.Select(igreja => igreja.Id).ToList();

        DateOnly inicioDoMes = new(request.Ano, request.Mes, 1);
        DateOnly fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

        IReadOnlyList<TotalDeLancamentos> totais = await relatorioRepository.SomarAsync(igrejaIds, inicioDoMes, fimDoMes, cancellationToken);

        List<ResumoDaIgrejaResponse> resumos = igrejas
            .Select(igreja => Resumir(igreja, totais.Where(total => total.IgrejaId == igreja.Id).ToList()))
            .ToList();

        return new RelatorioConsolidadoResponse(
            matriz.Id,
            matriz.Nome,
            request.Ano,
            request.Mes,
            resumos.Sum(resumo => resumo.SaldoAnterior),
            resumos.Sum(resumo => resumo.TotalEntradas),
            resumos.Sum(resumo => resumo.TotalSaidas),
            resumos.Sum(resumo => resumo.SaldoFinal),
            resumos);
    }

    private static ResumoDaIgrejaResponse Resumir(Igreja igreja, IReadOnlyList<TotalDeLancamentos> totais)
    {
        decimal saldoAnterior = CalculoDoSaldo.SaldoAntesDoMes(totais);
        decimal totalEntradas = CalculoDoSaldo.EntradasDoMes(totais);
        decimal totalSaidas = CalculoDoSaldo.SaidasDoMes(totais);

        return new ResumoDaIgrejaResponse(
            igreja.Id,
            igreja.Nome,
            igreja.EhMatriz,
            saldoAnterior,
            totalEntradas,
            totalSaidas,
            saldoAnterior + totalEntradas - totalSaidas);
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
}