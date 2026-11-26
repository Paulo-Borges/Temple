using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Lancamentos;

internal static class RegrasDoLancamento
{
    public static IRuleBuilderOptions<T, decimal> ValorDoLancamento<T>(this IRuleBuilder<T, decimal> regra)
    {
        return regra
            .GreaterThan(0m)
            .LessThanOrEqualTo(Lancamento.ValorMaximo)
            .Must(valor => decimal.Round(valor, Lancamento.CasasDecimais) == valor)
            .WithMessage($"'{{PropertyName}}' deve ter no máximo {Lancamento.CasasDecimais} casas decimais.");
    }

    public static IRuleBuilderOptions<T, DateOnly> DataDoLancamento<T>(this IRuleBuilder<T, DateOnly> regra, TimeProvider timeProvider)
    {
        return regra
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime))
            .WithMessage("'{PropertyName}' não pode estar no futuro.");
    }

    public static IRuleBuilderOptions<T, string> DescricaoObrigatoria<T>(this IRuleBuilder<T, string> regra)
    {
        return regra
            .NotEmpty()
            .MaximumLength(Lancamento.TamanhoMaximoDescricao);
    }

    public static IRuleBuilderOptions<T, string?> Observacao<T>(this IRuleBuilder<T, string?> regra)
    {
        return regra.MaximumLength(Lancamento.TamanhoMaximoDescricao);
    }
}