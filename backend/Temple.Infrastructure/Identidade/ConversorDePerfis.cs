using Temple.Domain.Enums;

namespace Temple.Infrastructure.Identidade;

public static class ConversorDePerfis
{
    public static string ParaNomeDoPapel(Perfil perfil)
    {
        return perfil.ToString();
    }

    public static IReadOnlyList<Perfil> ParaPerfis(IEnumerable<string> nomesDosPapeis)
    {
        List<Perfil> perfis = [];

        foreach (string nomeDoPapel in nomesDosPapeis)
        {
            if (!Enum.TryParse(nomeDoPapel, ignoreCase: false, out Perfil perfil) || !Enum.IsDefined(perfil))
            {
                throw new InvalidOperationException($"O papel '{nomeDoPapel}' não corresponde a nenhum perfil.");
            }

            perfis.Add(perfil);
        }

        perfis.Sort();

        return perfis;
    }
}
