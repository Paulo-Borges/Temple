using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Lancamentos;

public sealed class EstornarLancamentoRequestValidator : AbstractValidator<EstornarLancamentoRequest>
{
    public EstornarLancamentoRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Motivo)
            .NotEmpty()
            .MaximumLength(Lancamento.TamanhoMaximoMotivoDoEstorno);
    }
}