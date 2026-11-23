using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Membros;

public sealed class PromoverCargoRequestValidator : AbstractValidator<PromoverCargoRequest>
{
    public PromoverCargoRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.NovoCargo)
            .IsInEnum();

        RuleFor(request => request.DataPromocao)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime))
            .WithMessage("'{PropertyName}' não pode estar no futuro.");

        RuleFor(request => request.Observacao)
            .MaximumLength(HistoricoCargo.TamanhoMaximoObservacao);
    }
}