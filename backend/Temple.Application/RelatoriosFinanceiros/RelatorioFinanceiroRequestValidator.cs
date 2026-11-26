using FluentValidation;

namespace Temple.Application.RelatoriosFinanceiros;

public sealed class RelatorioFinanceiroRequestValidator : AbstractValidator<RelatorioFinanceiroRequest>
{
    private const int AnoMinimo = 1900;

    public RelatorioFinanceiroRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.Ano)
            .GreaterThanOrEqualTo(AnoMinimo);

        RuleFor(request => request.Mes)
            .InclusiveBetween(1, 12)
            .Must((request, mes) => NaoEstaNoFuturo(request.Ano, mes, timeProvider))
            .WithMessage("O mês do relatório não pode estar no futuro.");
    }

    // O mês atual é permitido: o relatório mostra o movimento até agora.
    private static bool NaoEstaNoFuturo(int ano, int mes, TimeProvider timeProvider)
    {
        DateTimeOffset agora = timeProvider.GetLocalNow();

        return ano < agora.Year || (ano == agora.Year && mes <= agora.Month);
    }
}