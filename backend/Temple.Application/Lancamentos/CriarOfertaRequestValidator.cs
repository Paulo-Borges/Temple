using FluentValidation;

namespace Temple.Application.Lancamentos;

public sealed class CriarOfertaRequestValidator : AbstractValidator<CriarOfertaRequest>
{
    public CriarOfertaRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.Data).DataDoLancamento(timeProvider);

        RuleFor(request => request.Valor).ValorDoLancamento();

        RuleFor(request => request.FormaPagamento)
            .IsInEnum();

        // Nulo é oferta anônima; um Guid vazio é erro de quem chamou.
        RuleFor(request => request.MembroId)
            .NotEqual(Guid.Empty);

        RuleFor(request => request.Observacao).Observacao();
    }
}