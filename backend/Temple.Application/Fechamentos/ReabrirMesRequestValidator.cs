using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Fechamentos;

public sealed class ReabrirMesRequestValidator : AbstractValidator<ReabrirMesRequest>
{
    public ReabrirMesRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.Ano).AnoDoFechamento();

        RuleFor(request => request.Mes).MesDoFechamento();

        RuleFor(request => request.Motivo)
            .NotEmpty()
            .MaximumLength(FechamentoMensal.TamanhoMaximoMotivoDaReabertura);
    }
}