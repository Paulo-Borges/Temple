using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Igrejas;

internal static class RegrasDaIgreja
{
    public static IRuleBuilderOptions<T, string> NomeDaIgreja<T>(this IRuleBuilder<T, string> regra)
    {
        return regra
            .NotEmpty()
            .MaximumLength(Igreja.TamanhoMaximoNome);
    }

    public static IRuleBuilderOptions<T, string> TelefoneDaIgreja<T>(this IRuleBuilder<T, string> regra)
    {
        return regra
            .NotEmpty()
            .Matches(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$")
            .WithMessage("'{PropertyName}' deve ter DDD e 8 ou 9 dígitos (ex.: (11) 3333-4444).");
    }
}
