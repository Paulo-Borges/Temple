using System.Globalization;
using Temple.Domain.Enums;

namespace Temple.Application.Certificados;

internal static class TextosDoCertificado
{
    private static readonly CultureInfo PortuguesDoBrasil = new("pt-BR");

    public static string NomeDoCargo(TipoCargo cargo)
    {
        return cargo switch
        {
            TipoCargo.Membro => "Membro",
            TipoCargo.Cooperador => "Cooperador",
            TipoCargo.Diacono => "Diácono",
            TipoCargo.Presbitero => "Presbítero",
            TipoCargo.Evangelista => "Evangelista",
            TipoCargo.Missionario => "Missionário",
            TipoCargo.Pastor => "Pastor",
            _ => throw new ArgumentOutOfRangeException(nameof(cargo), cargo, "Cargo sem nome definido.")
        };
    }

    public static string DataPorExtenso(DateOnly data)
    {
        return data.ToString("d 'de' MMMM 'de' yyyy", PortuguesDoBrasil);
    }

        // "filha" concorda com "a criança", então o texto serve para menino e menina.
    public static string Filiacao(string? nomeDoPai, string? nomeDaMae)
    {
        return (nomeDoPai, nomeDaMae) switch
        {
            (string pai, string mae) => $"filha de {pai} e de {mae}",
            (string pai, null) => $"filha de {pai}",
            (null, string mae) => $"filha de {mae}",
            (null, null) => throw new InvalidOperationException("A apresentação precisa ter o nome do pai ou da mãe.")
        };
    }

    public static string NomeDoArquivo(int numero)
    {
        return $"certificado-{numero:D6}.pdf";
    }
}