using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Apresentacoes;

internal static class RegrasDaApresentacao
{
    public const string MensagemDosPais = "Informe o nome do pai, o da mãe ou os dois.";

    public static IRuleBuilderOptions<T, string> NomeObrigatorio<T>(this IRuleBuilder<T, string> regra)
    {
        return regra
            .NotEmpty()
            .MaximumLength(ApresentacaoCrianca.TamanhoMaximoNome);
    }

    public static IRuleBuilderOptions<T, string?> NomeOpcional<T>(this IRuleBuilder<T, string?> regra)
    {
        return regra.MaximumLength(ApresentacaoCrianca.TamanhoMaximoNome);
    }

    public static IRuleBuilderOptions<T, string> Oficiante<T>(this IRuleBuilder<T, string> regra)
    {
        return regra
            .NotEmpty()
            .MaximumLength(ApresentacaoCrianca.TamanhoMaximoOficiante);
    }

    public static IRuleBuilderOptions<T, DateOnly> AteHoje<T>(this IRuleBuilder<T, DateOnly> regra, TimeProvider timeProvider)
    {
        return regra
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime))
            .WithMessage("'{PropertyName}' não pode estar no futuro.");
    }

    public static bool TemAlgumDosPais(string? nomeDoPai, string? nomeDaMae)
    {
        return !string.IsNullOrWhiteSpace(nomeDoPai) || !string.IsNullOrWhiteSpace(nomeDaMae);
    }
}