using FluentValidation;
using Temple.Domain.Enums;

namespace Temple.Application.Usuarios;

public static class RegrasDoUsuario
{
    public const int TamanhoMaximoNome = 150;
    public const int TamanhoMaximoEmail = 256;

    public static IRuleBuilderOptions<T, string> NomeDoUsuario<T>(this IRuleBuilder<T, string> regra)
    {
        return regra
            .NotEmpty()
            .MaximumLength(TamanhoMaximoNome);
    }

    public static IRuleBuilderOptions<T, IEnumerable<Perfil>> PerfisDoUsuario<T>(this IRuleBuilder<T, IReadOnlyList<Perfil>> regra)
    {
        return regra
            .NotEmpty()
            .Must(perfis => perfis.Distinct().Count() == perfis.Count)
            .WithMessage("'{PropertyName}' não pode ter perfis repetidos.")
            .ForEach(perfil => perfil.IsInEnum());
    }
}
