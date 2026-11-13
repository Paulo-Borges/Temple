namespace Temple.Domain.ValueObjects;

public sealed record Endereco
{
    private const int TamanhoMaximoLogradouro = 150;
    private const int TamanhoMaximoNumero = 10;
    private const int TamanhoMaximoComplemento = 100;
    private const int TamanhoMaximoBairro = 100;
    private const int TamanhoMaximoCidade = 100;

    private static readonly HashSet<string> UfsValidas =
    [
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
        "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
        "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
    ];

    public string Logradouro { get; }
    public string Numero { get; }
    public string? Complemento { get; }
    public string Bairro { get; }
    public string Cidade { get; }
    public string Uf { get; }
    public string Cep { get; }

    public Endereco(string logradouro, string numero, string? complemento, string bairro, string cidade, string uf, string cep)
    {
        Logradouro = ValidarTexto(logradouro, TamanhoMaximoLogradouro, nameof(logradouro));
        Numero = ValidarTexto(numero, TamanhoMaximoNumero, nameof(numero));
        Complemento = ValidarComplemento(complemento);
        Bairro = ValidarTexto(bairro, TamanhoMaximoBairro, nameof(bairro));
        Cidade = ValidarTexto(cidade, TamanhoMaximoCidade, nameof(cidade));
        Uf = ValidarUf(uf);
        Cep = ValidarCep(cep);
    }

    private static string ValidarTexto(string valor, int tamanhoMaximo, string nomeDoCampo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valor, nomeDoCampo);

        string valorLimpo = valor.Trim();

        if (valorLimpo.Length > tamanhoMaximo)
        {
            throw new ArgumentException($"O campo deve ter no máximo {tamanhoMaximo} caracteres.", nomeDoCampo);
        }

        return valorLimpo;
    }

    private static string? ValidarComplemento(string? complemento)
    {
        if (string.IsNullOrWhiteSpace(complemento))
        {
            return null;
        }

        return ValidarTexto(complemento, TamanhoMaximoComplemento, nameof(complemento));
    }

    private static string ValidarUf(string uf)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uf);

        string ufLimpa = uf.Trim().ToUpperInvariant();

        if (!UfsValidas.Contains(ufLimpa))
        {
            throw new ArgumentException($"A UF '{ufLimpa}' não é válida.", nameof(uf));
        }

        return ufLimpa;
    }

    private static string ValidarCep(string cep)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cep);

        string semMascara = cep.Trim()
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(".", string.Empty, StringComparison.Ordinal);

        if (semMascara.Length != 8 || !semMascara.All(char.IsAsciiDigit))
        {
            throw new ArgumentException("O CEP deve ter 8 números.", nameof(cep));
        }

        return semMascara;
    }
}
