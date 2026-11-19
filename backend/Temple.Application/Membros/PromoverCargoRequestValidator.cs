using FluentValidation;

namespace Temple.Application.Membros;

public sealed class PromoverCargoRequestValidator : AbstractValidator<PromoverCargoRequest>
{
    public PromoverCargoRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.NovoCargo)
            .IsInEnum();

        RuleFor(request => request.DataPromocao)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("'{PropertyName}' não pode estar no futuro.");
    }
}
