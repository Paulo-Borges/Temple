using FluentValidation;
using Temple.Application.Comum;

namespace Temple.Application.Membros;

public sealed class AtualizarMembroRequestValidator : AbstractValidator<AtualizarMembroRequest>
{
    public AtualizarMembroRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Nome).NomeDoMembro();

        RuleFor(request => request.Endereco)
            .NotNull()
            .SetValidator(new EnderecoDtoValidator());

        RuleFor(request => request.Grupo)
            .IsInEnum();
    }
}
