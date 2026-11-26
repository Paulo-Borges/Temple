using FluentValidation;

namespace Temple.Application.Lancamentos;

public sealed class CriarDizimoRequestValidator : AbstractValidator<CriarDizimoRequest>
{
    public CriarDizimoRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.MembroId)
            .NotEmpty();

        RuleFor(request => request.Data).DataDoLancamento(timeProvider);

        RuleFor(request => request.Valor).ValorDoLancamento();

        RuleFor(request => request.FormaPagamento)
            .IsInEnum();

        RuleFor(request => request.Observacao).Observacao();
    }
}