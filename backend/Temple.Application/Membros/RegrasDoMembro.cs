using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Membros;

internal static class RegrasDoMembro
{
    public static IRuleBuilderOptions<T, string> NomeDoMembro<T>(this IRuleBuilder<T, string> regra)
    {
        return regra
            .NotEmpty()
            .MaximumLength(Membro.TamanhoMaximoNome);
    }
}
