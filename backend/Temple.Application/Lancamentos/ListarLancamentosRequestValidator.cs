using FluentValidation;

namespace Temple.Application.Lancamentos;

public sealed class ListarLancamentosRequestValidator : AbstractValidator<ListarLancamentosRequest>
{
    private const int TamanhoMaximoPagina = 100;

    public ListarLancamentosRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Ate)
            .GreaterThanOrEqualTo(request => request.De)
            .When(request => request.De is not null && request.Ate is not null)
            .WithMessage("'Até' não pode ser anterior a 'De'.");

        RuleFor(request => request.Tipo)
            .IsInEnum();

        RuleFor(request => request.Pagina)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, TamanhoMaximoPagina);
    }
}