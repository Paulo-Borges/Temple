namespace Temple.Application.Comprovantes;

// Escreve um valor em reais por extenso, como num cheque: 1.250,50 → "mil duzentos e cinquenta reais e cinquenta centavos".
internal static class ValorPorExtenso
{
    private static readonly string[] Unidades =
    [
        "", "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove",
        "dez", "onze", "doze", "treze", "quatorze", "quinze", "dezesseis", "dezessete", "dezoito", "dezenove"
    ];

    private static readonly string[] Dezenas =
    [
        "", "", "vinte", "trinta", "quarenta", "cinquenta", "sessenta", "setenta", "oitenta", "noventa"
    ];

    private static readonly string[] Centenas =
    [
        "", "cento", "duzentos", "trezentos", "quatrocentos", "quinhentos", "seiscentos", "setecentos", "oitocentos", "novecentos"
    ];

    // O nome de cada grupo de três dígitos, da direita para a esquerda. O primeiro grupo não tem nome.
    private static readonly (string Singular, string Plural)[] Escalas =
    [
        ("", ""),
        ("mil", "mil"),
        ("milhão", "milhões"),
        ("bilhão", "bilhões"),
        ("trilhão", "trilhões"),
        ("quatrilhão", "quatrilhões")
    ];

    public static string EmReais(decimal valor)
    {
        if (valor <= 0m || decimal.Round(valor, 2) != valor)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), valor, "O valor precisa ser positivo e ter no máximo 2 casas decimais.");
        }

        long reais = decimal.ToInt64(decimal.Truncate(valor));
        int centavos = decimal.ToInt32((valor - decimal.Truncate(valor)) * 100);

        List<string> partes = [];

        if (reais > 0)
        {
            partes.Add(EscreverReais(reais));
        }

        if (centavos > 0)
        {
            partes.Add(centavos == 1 ? "um centavo" : $"{Escrever(centavos)} centavos");
        }

        return string.Join(" e ", partes);
    }

    private static string EscreverReais(long reais)
    {
        if (reais == 1)
        {
            return "um real";
        }

        // "um milhão de reais", "dois bilhões de reais": o "de" entra quando o valor termina em milhão ou maior, sem nada depois.
        string preposicao = reais % 1_000_000 == 0 ? " de" : "";

        return $"{Escrever(reais)}{preposicao} reais";
    }

    private static string Escrever(long numero)
    {
        List<int> grupos = [];

        for (long resto = numero; resto > 0; resto /= 1000)
        {
            grupos.Add(Convert.ToInt32(resto % 1000));
        }

        List<(int Valor, string Texto)> partes = [];

        for (int escala = grupos.Count - 1; escala >= 0; escala--)
        {
            int grupo = grupos[escala];

            if (grupo == 0)
            {
                continue;
            }

            partes.Add((grupo, EscreverGrupo(grupo, escala)));
        }

        string texto = partes[0].Texto;

        for (int i = 1; i < partes.Count; i++)
        {
            // O "e" entra antes do último grupo quando ele é menor que cem ou é uma centena redonda: "mil e cem", "mil e vinte".
            bool ultimo = i == partes.Count - 1;
            bool usaE = ultimo && (partes[i].Valor < 100 || partes[i].Valor % 100 == 0);

            texto += (usaE ? " e " : " ") + partes[i].Texto;
        }

        return texto;
    }

    private static string EscreverGrupo(int grupo, int escala)
    {
        // "mil", e não "um mil".
        if (escala == 1 && grupo == 1)
        {
            return "mil";
        }

        string centena = EscreverCentena(grupo);

        if (escala == 0)
        {
            return centena;
        }

        (string singular, string plural) = Escalas[escala];

        return $"{centena} {(grupo == 1 ? singular : plural)}";
    }

    // De 1 a 999.
    private static string EscreverCentena(int numero)
    {
        if (numero == 100)
        {
            return "cem";
        }

        List<string> partes = [];
        int centena = numero / 100;
        int resto = numero % 100;

        if (centena > 0)
        {
            partes.Add(Centenas[centena]);
        }

        if (resto > 0 && resto < 20)
        {
            partes.Add(Unidades[resto]);
        }
        else if (resto >= 20)
        {
            int dezena = resto / 10;
            int unidade = resto % 10;

            partes.Add(unidade == 0 ? Dezenas[dezena] : $"{Dezenas[dezena]} e {Unidades[unidade]}");
        }

        return string.Join(" e ", partes);
    }
}