using FluentValidation;

namespace Temple.Application.Lancamentos;

public sealed class CriarDespesaRequestValidator : AbstractValidator<CriarDespesaRequest>
{
    public CriarDespesaRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.Categoria)
            .IsInEnum();

        RuleFor(request => request.Data).DataDoLancamento(timeProvider);

        RuleFor(request => request.Valor).ValorDoLancamento();

        RuleFor(request => request.FormaPagamento)
            .IsInEnum();

        RuleFor(request => request.Descricao).DescricaoObrigatoria();
    }
}