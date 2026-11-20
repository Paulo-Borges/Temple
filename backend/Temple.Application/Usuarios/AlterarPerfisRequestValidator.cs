using FluentValidation;

namespace Temple.Application.Usuarios;

public sealed class AlterarPerfisRequestValidator : AbstractValidator<AlterarPerfisRequest>
{
    public AlterarPerfisRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Perfis).PerfisDoUsuario();
    }
}
