using FluentValidation;
using Temple.Application.Comum;

namespace Temple.Application.Igrejas;

public sealed class AtualizarIgrejaRequestValidator : AbstractValidator<AtualizarIgrejaRequest>
{
    public AtualizarIgrejaRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Nome).NomeDaIgreja();

        RuleFor(request => request.Telefone).TelefoneDaIgreja();

        RuleFor(request => request.Endereco)
            .NotNull()
            .SetValidator(new EnderecoDtoValidator());
    }
}
