using FluentValidation;

namespace Temple.Application.Fechamentos;

internal static class RegrasDoFechamento
{
    private const int AnoMinimo = 1900;

    public static IRuleBuilderOptions<T, int> AnoDoFechamento<T>(this IRuleBuilder<T, int> regra)
    {
        return regra.GreaterThanOrEqualTo(AnoMinimo);
    }

    public static IRuleBuilderOptions<T, int> MesDoFechamento<T>(this IRuleBuilder<T, int> regra)
    {
        return regra.InclusiveBetween(1, 12);
    }
}