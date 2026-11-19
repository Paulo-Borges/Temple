using FluentValidation;
using Temple.Application.Comum;

namespace Temple.Application.Igrejas;

public sealed class CriarIgrejaRequestValidator : AbstractValidator<CriarIgrejaRequest>
{
    public CriarIgrejaRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Nome).NomeDaIgreja();

        RuleFor(request => request.Cnpj)
            .NotEmpty()
            .Matches(@"^[A-Za-z0-9]{2}\.?[A-Za-z0-9]{3}\.?[A-Za-z0-9]{3}/?[A-Za-z0-9]{4}-?\d{2}$")
            .WithMessage("'{PropertyName}' deve ter 12 letras ou números e 2 dígitos verificadores (ex.: 11.222.333/0001-81).");

        RuleFor(request => request.Telefone).TelefoneDaIgreja();

        RuleFor(request => request.DataFundacao)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("'{PropertyName}' não pode estar no futuro.");

        RuleFor(request => request.Endereco)
            .NotNull()
            .SetValidator(new EnderecoDtoValidator());
    }
}

