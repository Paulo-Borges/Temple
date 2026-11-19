using FluentValidation;
using Temple.Application.Comum;

namespace Temple.Application.Membros;

public sealed class CriarMembroRequestValidator : AbstractValidator<CriarMembroRequest>
{
    public CriarMembroRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Nome).NomeDoMembro();

        RuleFor(request => request.DataNascimento)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("'{PropertyName}' não pode estar no futuro.");

        RuleFor(request => request.Endereco)
            .NotNull()
            .SetValidator(new EnderecoDtoValidator());

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.CargoAtual)
            .IsInEnum();

        RuleFor(request => request.Grupo)
            .IsInEnum();
    }
}
