using FluentValidation;

namespace Temple.Application.Fechamentos;

public sealed class FecharMesRequestValidator : AbstractValidator<FecharMesRequest>
{
    public FecharMesRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.Ano).AnoDoFechamento();

        RuleFor(request => request.Mes).MesDoFechamento();
    }
}