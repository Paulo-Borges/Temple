using FluentValidation;
using Temple.Domain.ValueObjects;

namespace Temple.Application.Comum;

public sealed class EnderecoDtoValidator : AbstractValidator<EnderecoDto>
{
    public EnderecoDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(endereco => endereco.Logradouro)
            .NotEmpty()
            .MaximumLength(Endereco.TamanhoMaximoLogradouro);

        RuleFor(endereco => endereco.Numero)
            .NotEmpty()
            .MaximumLength(Endereco.TamanhoMaximoNumero);

        RuleFor(endereco => endereco.Complemento)
            .MaximumLength(Endereco.TamanhoMaximoComplemento);

        RuleFor(endereco => endereco.Bairro)
            .NotEmpty()
            .MaximumLength(Endereco.TamanhoMaximoBairro);

        RuleFor(endereco => endereco.Cidade)
            .NotEmpty()
            .MaximumLength(Endereco.TamanhoMaximoCidade);

        RuleFor(endereco => endereco.Uf)
            .NotEmpty()
            .Must(Endereco.EhUfValida)
            .WithMessage("'{PropertyName}' não é uma UF válida.");

        RuleFor(endereco => endereco.Cep)
            .NotEmpty()
            .Matches(@"^\d{5}-?\d{3}$")
            .WithMessage("'{PropertyName}' deve ter 8 números (ex.: 01001-000).");
    }
}
