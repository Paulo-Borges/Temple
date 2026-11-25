using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Batismos;

public sealed class BatismoRequestValidator : AbstractValidator<BatismoRequest>
{
    public BatismoRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.DataBatismo)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime))
            .WithMessage("'{PropertyName}' não pode estar no futuro.");

        RuleFor(request => request.Oficiante)
            .NotEmpty()
            .MaximumLength(Batismo.TamanhoMaximoOficiante);

        RuleFor(request => request.Local)
            .MaximumLength(Batismo.TamanhoMaximoLocal);
    }
}